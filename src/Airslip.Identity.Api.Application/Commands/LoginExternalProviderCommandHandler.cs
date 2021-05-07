using Airslip.Common.Contracts;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Security.Jwt;
using Airslip.Yapily.Client.Contracts;
using MediatR;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Commands
{
    public class LoginExternalProviderCommandHandler : IRequestHandler<LoginExternalProviderCommand, IResponse>
    {
        private readonly IUserService _userService;
        private readonly IYapilyClient _yapilyApis;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger _logger;

        public LoginExternalProviderCommandHandler(
            IUserService userService,
            IYapilyClient yapilyApis,
            IOptions<JwtSettings> jwtSettingsOptions)
        {
            _userService = userService;
            _yapilyApis = yapilyApis;
            _jwtSettings = jwtSettingsOptions.Value;
            _logger = Log.Logger;
        }

        public async Task<IResponse> Handle(LoginExternalProviderCommand command, CancellationToken cancellationToken)
        {
            _logger.ForContext(nameof(command.Email), command.Email);

            User? user = await _userService.GetByEmail(command.Email);

            if (user is null)
            {
                IYapilyResponse response =
                    await _yapilyApis.CreateUser(command.Email, command.ReferenceId, cancellationToken);

                switch (response)
                {
                    case YapilyApiResponseError apiError:
                        switch (apiError.Error.Code)
                        {
                            case (int) HttpStatusCode.Conflict:
                                return new ConflictResponse(nameof(command.Email), command.Email,
                                    "User already exists");
                            default:
                                _logger.Fatal("UNHANDLED_YAPILY_ERROR. ErrorMessage : {ErrorMessage}",
                                    apiError.Error.Message);
                                throw new InvalidOperationException();
                        }

                    case YapilyUser yapilyUser:

                        if (yapilyUser.IsInvalid())
                        {
                            await _yapilyApis.DeleteUser(yapilyUser.Uuid!, cancellationToken);
                            return new ResourceNotFound(nameof(User), "Unable to create with all the required fields");
                        }

                        await _userService.Create(
                            new User(
                                yapilyUser.Uuid!,
                                yapilyUser.ApplicationUuid!,
                                yapilyUser.ApplicationUserId!,
                                yapilyUser.ReferenceId!,
                                yapilyUser.InstitutionConsents.Select(yapilyInstitutionConsent =>
                                    new UserInstitution(yapilyInstitutionConsent.InstitutionId!)).ToList()));

                        user = await _userService.Get(yapilyUser.Uuid!);
                        
                        _logger.Information("User {UserId} successfully logged in with {ExternalProvider}", 
                            user.Id, 
                            command.Provider);

                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            string jwtBearerToken = JwtBearerToken.Generate(
                _jwtSettings.Key,
                _jwtSettings.Audience,
                _jwtSettings.Issuer,
                _jwtSettings.ExpiresTime,
                user.Id);

            bool hasAddedInstitution = user.Institutions.Count > 0;

            return new AuthenticatedUserResponse(jwtBearerToken, hasAddedInstitution);
        }
    }
}
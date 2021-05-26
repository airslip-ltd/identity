using Airslip.Common.Contracts;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Security.Jwt;
using Airslip.Yapily.Client.Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IResponse>
    {
        private readonly IUserService _userService;
        private readonly IYapilyClient _yapilyApis;
        private readonly JwtSettings _jwtSettings;
        private readonly IUserManagerService _userManagerService;
        private readonly ILogger _logger;

        public RegisterUserCommandHandler(
            IUserService userService,
            IYapilyClient yapilyApis,
            IOptions<JwtSettings> jwtSettingsOptions,
            IUserManagerService userManagerService)
        {
            _userService = userService;
            _yapilyApis = yapilyApis;
            _jwtSettings = jwtSettingsOptions.Value;
            _userManagerService = userManagerService;
            _logger = Log.Logger;
        }

        public async Task<IResponse> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            //string encryptedEmail = Cryptography.GenerateSHA256String(command.Email);

            ILogger logger = Log
                .ForContext(nameof(command.Email), command.Email);

            IYapilyResponse response =
                await _yapilyApis.CreateUser(command.Email, command.ReferenceId, cancellationToken);

            switch (response)
            {
                case YapilyApiResponseError apiError:
                    switch (apiError.Error.Code)
                    {
                        case (int) HttpStatusCode.Conflict:
                            return new ConflictResponse(nameof(command.Email), command.Email, "User already exists");
                        default:
                            logger.Fatal("UNHANDLED_YAPILY_ERROR. ErrorMessage : {ErrorMessage}",
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

                    IdentityResult result = await _userManagerService.Create(command.Email, command.Password);

                    if (result.Succeeded is false)
                        return result.Errors.First().Code switch
                        {
                            "DuplicateUserName" => new ConflictResponse(nameof(command.Email), command.Email,
                                "User already exists"),
                            _ => new ErrorResponse(result.Errors.First().Code,
                                result.Errors.First().Description)
                        };

                    User user = await _userService.Get(yapilyUser.Uuid!);

                    _logger.Information("User {UserId} successfully registered", user.Id);

                    string jwtBearerToken = JwtBearerToken.Generate(
                        _jwtSettings.Key,
                        _jwtSettings.Audience,
                        _jwtSettings.Issuer,
                        _jwtSettings.ExpiresTime,
                        user.Id);

                    bool hasAddedInstitution = user.Institutions.Count > 0;

                    return new AuthenticatedUserResponse(jwtBearerToken, hasAddedInstitution);

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
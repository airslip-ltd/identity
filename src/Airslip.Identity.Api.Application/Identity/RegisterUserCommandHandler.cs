using Airslip.Common.Auth.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Contracts;
using Airslip.Common.Types.Extensions;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Yapily.Client.Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IResponse>
    {
        private readonly IUserLoginService _userLoginService;
        private readonly IUserService _userService;
        private readonly IYapilyClient _yapilyApis;
        private readonly IUserManagerService _userManagerService;
        private readonly ILogger _logger;
        private readonly IUserProfileService _userProfileService;

        public RegisterUserCommandHandler(
            IUserLoginService userLoginService,
            IUserService userService,
            IYapilyClient yapilyApis,
            IUserManagerService userManagerService,
            IUserProfileService userProfileService)
        {
            _userLoginService = userLoginService;
            _userService = userService;
            _yapilyApis = yapilyApis;
            _userManagerService = userManagerService;
            _userProfileService = userProfileService;
            _logger = Log.Logger;
        }

        public async Task<IResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            IdentityResult result = await _userManagerService.Create(request.Email, request.Password);

            if (result.Succeeded is false)
                return result.Errors.First().Code switch
                {
                    "DuplicateUserName" => new ConflictResponse(
                        nameof(request.Email),
                        request.Email,
                        "User already exists"),
                    "PasswordRequiresUpper" => new IncorrectPasswordResponse("Passwords must have at least one non alphanumeric character."),
                    _ => new ErrorResponse(
                        result.Errors.First().Code,
                        result.Errors.First().Description)
                };

            User user = await _userService.Create(new User());

            IYapilyResponse response =
                await _yapilyApis.CreateUser(user.Id, request.ReferenceId, cancellationToken);

            switch (response)
            {
                case YapilyApiResponseError apiError:
                    switch (apiError.Error.Code)
                    {
                        case (int) HttpStatusCode.Conflict:
                            return new ConflictResponse(nameof(request.Email), request.Email, "User already exists");
                        default:
                            _logger.Fatal("UNHANDLED_YAPILY_ERROR. ErrorMessage : {ErrorMessage}",
                                apiError.Error.Message);
                            throw new InvalidOperationException();
                    }

                case YapilyUser yapilyUser:
                    string yapilyUserId = yapilyUser.Uuid;
                    string yapilyApplicationId = yapilyUser.ApplicationUuid;
                    string yapilyReferenceId = yapilyUser.ReferenceId;

                    if (yapilyUser.IsInvalid())
                    {
                        await _yapilyApis.DeleteUser(yapilyUserId, cancellationToken);
                        return new ResourceNotFound(nameof(User), "Unable to create with all the required fields");
                    }
                    
                    user.AddOpenBankingProvider( new OpenBankingProvider("Yapily", yapilyUserId, yapilyApplicationId, yapilyReferenceId));
                    
                    await _userService.Update(user);
                    
                    await _userProfileService.Create(new UserProfile(user.Id, request.Email));

                    _logger.Information("User {UserId} successfully registered", user.Id);

                    return await _userLoginService
                        .GenerateUserResponse(user, true, yapilyUserId, request.DeviceId);

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
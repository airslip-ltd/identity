using Airslip.Common.Contracts;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Yapily.Client.Contracts;
using MediatR;
using Serilog;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class LoginExternalProviderCommandHandler : IRequestHandler<LoginExternalProviderCommand, IResponse>
    {
        private readonly IUserLoginService _userLoginService;
        private readonly IUserService _userService;
        private readonly IYapilyClient _yapilyApis;
        private readonly ILogger _logger;
        private readonly IUserProfileService _userProfileService;

        public LoginExternalProviderCommandHandler(
            IUserLoginService userLoginService,
            IUserService userService,
            IYapilyClient yapilyApis,
            IUserProfileService userProfileService)
        {
            _userLoginService = userLoginService;
            _userService = userService;
            _yapilyApis = yapilyApis;
            _userProfileService = userProfileService;
            _logger = Log.Logger;
        }

        public async Task<IResponse> Handle(LoginExternalProviderCommand request, CancellationToken cancellationToken)
        {
            UserProfile? userProfile = await _userProfileService.GetByEmail(request.Email);
            bool isNewUser = userProfile is null;

            User? user = isNewUser 
                ? await _userService.Create(new User())
                : await _userService.Get(userProfile!.UserId);
            
            string yapilyUserId = user!.GetOpenBankingProviderId("Yapily") ?? string.Empty;

            if (isNewUser)
            {
                await _userProfileService.Create(new UserProfile(user.Id, request.Email));

                IYapilyResponse response =
                    await _yapilyApis.CreateUser(user.Id, request.ReferenceId, cancellationToken);

                switch (response)
                {
                    case YapilyApiResponseError apiError:
                        switch (apiError.Error.Code)
                        {
                            case (int) HttpStatusCode.Conflict:
                                return new ConflictResponse(nameof(request.Email), request.Email,
                                    "User already exists");
                            default:
                                _logger.Fatal("UNHANDLED_YAPILY_ERROR. ErrorMessage : {ErrorMessage}",
                                    apiError.Error.Message);
                                throw new InvalidOperationException();
                        }

                    case YapilyUser yapilyUser:
                        yapilyUserId = yapilyUser.Uuid;

                        if (yapilyUser.IsInvalid())
                        {
                            await _yapilyApis.DeleteUser(yapilyUserId, cancellationToken);
                            return new ResourceNotFound(nameof(User), "Unable to create with all the required fields");
                        }
                        
                        _logger.Information("User {UserId} successfully logged in with {ExternalProvider}",
                            user.Id,
                            request.Provider);

                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            return await _userLoginService.GenerateUserResponse(user, isNewUser, yapilyUserId, request.DeviceId);
        }
    }
}
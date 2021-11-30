using Airslip.Common.Types.Interfaces;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
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
        private readonly IYapilyClient _yapilyClient;
        private readonly ILogger _logger;

        public LoginExternalProviderCommandHandler(
            IUserLoginService userLoginService,
            IUserService userService,
            IYapilyClient yapilyClient)
        {
            _userLoginService = userLoginService;
            _userService = userService;
            _yapilyClient = yapilyClient;
            _logger = Log.Logger;
        }

        public async Task<IResponse> Handle(LoginExternalProviderCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userService.GetByEmail(request.Email);
            bool isNewUser = user is null;

            user = isNewUser 
                ? await _userService.Create(new User(request.Email, request.FirstName, request.LastName))
                : await _userService.Get(user!.Id);
            
            string yapilyUserId = user!.GetOpenBankingProviderId("Yapily") ?? string.Empty;

            if (!isNewUser)
                return await _userLoginService.GenerateUserResponse(user, isNewUser, yapilyUserId, request.DeviceId);
            
            IYapilyResponse response =
                await _yapilyClient.CreateUser(user.Id, request.ReferenceId, cancellationToken);

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
                        await _yapilyClient.DeleteUser(yapilyUserId, cancellationToken);
                        return new ResourceNotFound(nameof(User), "Unable to create with all the required fields");
                    }
                        
                    _logger.Information("User {UserId} successfully logged in with {ExternalProvider}",
                        user.Id,
                        request.Provider);

                    break;
                default:
                    throw new InvalidOperationException();
            }

            return await _userLoginService.GenerateUserResponse(user, isNewUser, yapilyUserId, request.DeviceId);
        }
    }
}
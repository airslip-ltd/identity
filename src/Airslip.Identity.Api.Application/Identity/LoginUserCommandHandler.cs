using Airslip.Common.Auth.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Contracts;
using Airslip.Common.Types.Extensions;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Contracts;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Identity.MongoDb.Contracts.Identity;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class GenerateJwtBearerTokenCommandHandler : IRequestHandler<LoginUserCommand, IResponse>
    {
        private readonly ITokenService<UserToken, GenerateUserToken> _tokenService;
        private readonly IUserService _userService;
        private readonly IUserProfileService _userProfileService;
        private readonly IUserManagerService _userManagerService;
        private readonly ILogger _logger;

        public GenerateJwtBearerTokenCommandHandler(
            ITokenService<UserToken, GenerateUserToken> tokenService,
            IUserService userService,
            IUserProfileService userProfileService,
            IUserManagerService userManagerService)
        {
            _tokenService = tokenService;
            _userService = userService;
            _userProfileService = userProfileService;
            _userManagerService = userManagerService;
            _logger = Log.Logger;
        }

        public async Task<IResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser? applicationUser = await _userManagerService.FindByEmail(request.Email);

            if (applicationUser is null)
                return new NotFoundResponse(nameof(request.Email), request.Email, "New user");

            bool canLogin = await _userManagerService.TryToLogin(applicationUser, request.Password);
            
            if (!canLogin)
                return new IncorrectPasswordResponse();

            UserProfile? userprofile = await _userProfileService.GetByEmail(request.Email);
            if (userprofile == null)
                return new NotFoundResponse(nameof(request.Email), request.Email, "Unable to find user");

            User user = (await _userService.Get(userprofile.UserId))!;

            string? yapilyUserId = user.GetOpenBankingProviderId("Yapily");

            if (yapilyUserId is null)
                return new InvalidResource("YapilyUserId", "Doesn't exist");

            _logger.Information("User {UserId} successfully logged in", user.Id);

            GenerateUserToken generateUserToken = new(user.Id, yapilyUserId ?? "", "");

            NewToken newToken = _tokenService.GenerateNewToken(generateUserToken);

            string refreshToken = JwtBearerToken.GenerateRefreshToken();

            await _userService.UpdateRefreshToken(user.Id, request.DeviceId, refreshToken);

            return new AuthenticatedUserResponse(
                newToken.TokenValue,
                newToken.TokenExpiry?.ToUnixTimeMilliseconds() ?? 0,
                refreshToken,
                user.BiometricOn,
                false);
        }
    }
}
using Airslip.Common.Auth.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Contracts;
using Airslip.Common.Types.Extensions;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Identity.MongoDb.Contracts.Entities;
using Airslip.Identity.MongoDb.Contracts.Interfaces;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class UserLoginService : IUserLoginService
    {
        private readonly ITokenService<UserToken, GenerateUserToken> _tokenService;
        private readonly IUserService _userService;

        public UserLoginService(ITokenService<UserToken, GenerateUserToken> tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }
        
        public async Task<IResponse> GenerateRefreshToken(string userId, string deviceId, string currentToken)
        {
            User? user = await _userService.Get(userId);

            if (user is null)
                return new NotFoundResponse(userId, userId, "Unable to find user");
            
            if (!user.RefreshTokens.Contains(new RefreshToken(deviceId, currentToken)))
                return new NotFoundResponse(nameof(RefreshToken),
                    currentToken,
                    "An incorrect refresh token has been used for this device");


            return await GenerateUserResponse(user, false, deviceId: deviceId);
        }

        public async Task<IResponse> GenerateUserResponse(User user, bool isNewUser,
            string? yapilyUserId = null,
            string deviceId = "",
            string identity = "")
        {
            yapilyUserId ??= user.GetOpenBankingProviderId("Yapily");

            GenerateUserToken generateUserToken = new(user.Id, yapilyUserId ?? "", identity);

            NewToken newToken = _tokenService.GenerateNewToken(generateUserToken);
            string newRefreshToken = JwtBearerToken.GenerateRefreshToken();
            
            await _userService.UpdateRefreshToken(user.Id, deviceId, newRefreshToken);

            return new AuthenticatedUserResponse(
                newToken.TokenValue,
                newToken.TokenExpiry?.ToUnixTimeMilliseconds() ?? 0,
                newRefreshToken,
                user.BiometricOn, 
                isNewUser);
        }
    }
}
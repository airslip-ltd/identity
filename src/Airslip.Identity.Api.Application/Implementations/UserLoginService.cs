using Airslip.Common.Auth.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Types.Interfaces;
using Airslip.Common.Types.Failures;
using Airslip.Common.Utilities.Extensions;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using Airslip.Identity.Api.Contracts.Responses;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class UserLoginService : IUserLoginService
    {
        private readonly ITokenGenerationService<GenerateUserToken> _tokenService;
        private readonly IModelMapper<UserModel> _mapper;
        private readonly IIdentityContext _context;

        public UserLoginService(ITokenGenerationService<GenerateUserToken> tokenService, IModelMapper<UserModel> mapper, 
            IIdentityContext context)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _context = context;
        }
        
        public async Task<IResponse> GenerateRefreshToken(string userId, string deviceId, string currentToken)
        {
            User? user = await _context.GetEntity<User>(userId);

            if (user is null)
                return new NotFoundResponse(userId, userId, "Unable to find user");
            
            if (!user.RefreshTokens.Contains(new RefreshToken(deviceId, currentToken)))
                return new NotFoundResponse(nameof(RefreshToken),
                    currentToken,
                    "An incorrect refresh token has been used for this device");

            return await GenerateUserResponse(user, 
                false,
                null,
                deviceId);
        }

        public async Task<IResponse> GenerateUserResponse(User user, 
            bool isNewUser,
            string? yapilyUserId = null, 
            string deviceId = "")
        {
            yapilyUserId ??= user.GetOpenBankingProviderId("Yapily");

            GenerateUserToken generateUserToken = new(user.EntityId ?? string.Empty, 
                user.AirslipUserType,
                user.Id, 
                yapilyUserId ?? string.Empty);

            NewToken newToken = _tokenService.GenerateNewToken(generateUserToken);
            string newRefreshToken = JwtBearerToken.GenerateRefreshToken();
            
            await _context.UpdateOrReplaceRefreshToken(user.Id, deviceId, newRefreshToken);

            return new AuthenticatedUserResponse(
                newToken.TokenValue,
                newToken.TokenExpiry?.ToUnixTimeMilliseconds() ?? 0,
                newRefreshToken,
                isNewUser,
                _mapper.Create(user));
        }
    }
}
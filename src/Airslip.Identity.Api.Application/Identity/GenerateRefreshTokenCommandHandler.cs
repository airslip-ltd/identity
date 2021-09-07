using Airslip.Common.Auth.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Contracts;
using Airslip.Common.Types.Extensions;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Identity.MongoDb.Contracts;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class GenerateRefreshTokenCommandHandler : IRequestHandler<GenerateRefreshTokenCommand, IResponse>
    {
        private readonly ITokenService<UserToken, GenerateUserToken> _tokenService;
        private readonly IUserService _userService;

        public GenerateRefreshTokenCommandHandler(
            ITokenService<UserToken, GenerateUserToken> tokenService,
            IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        public async Task<IResponse> Handle(GenerateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userService.Get(request.UserId);

            if (user is null)
                return new NotFoundResponse(request.UserId, request.UserId, "Unable to find user");
            
            if (!user.RefreshTokens.Contains(new RefreshToken(request.DeviceId, request.Token)))
                return new NotFoundResponse(nameof(RefreshToken),
                    request.Token,
                    "An incorrect refresh token has been used for this device");

            string? yapilyUserId = user.GetOpenBankingProviderId("Yapily");

            GenerateUserToken generateUserToken = new(request.UserId, yapilyUserId ?? "", "");

            NewToken newToken = _tokenService.GenerateNewToken(generateUserToken);

            string newRefreshToken = JwtBearerToken.GenerateRefreshToken();
            
            await _userService.UpdateRefreshToken(request.UserId, request.DeviceId, newRefreshToken);

            return new AuthenticatedUserResponse(
                newToken.TokenValue,
                newToken.TokenExpiry?.ToUnixTimeMilliseconds() ?? 0,
                newRefreshToken,
                user.BiometricOn, 
                false);
        }
    }
}
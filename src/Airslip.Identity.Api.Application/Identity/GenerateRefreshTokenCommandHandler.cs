using Airslip.Common.Auth.Implementations;
using Airslip.Common.Auth.Models;
using Airslip.Common.Contracts;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Identity.MongoDb.Contracts;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class GenerateRefreshTokenCommandHandler : IRequestHandler<GenerateRefreshTokenCommand, IResponse>
    {
        private readonly IUserService _userService;
        private readonly JwtSettings _jwtSettings;

        public GenerateRefreshTokenCommandHandler(
            IUserService userService,
            IOptions<JwtSettings> jwtSettingsOptions)
        {
            _userService = userService;
            _jwtSettings = jwtSettingsOptions.Value;
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

            DateTime bearerTokenExpiryDate = JwtBearerToken.GetExpiry(_jwtSettings.ExpiresTime);
            
            string? yapilyUserId = user.GetOpenBankingProviderId("Yapily");

            if (yapilyUserId is null)
                return new InvalidResource("YapilyUserId", "Doesn't exist");

            string jwtBearerToken = JwtBearerToken.Generate(
                _jwtSettings.Key,
                _jwtSettings.Audience,
                _jwtSettings.Issuer,
                bearerTokenExpiryDate,
                JwtBearerToken.GetClaims(user.Id, yapilyUserId));

            string newRefreshToken = JwtBearerToken.GenerateRefreshToken();
            await _userService.UpdateRefreshToken(request.UserId, request.DeviceId, newRefreshToken);

            return new AuthenticatedUserResponse(
                jwtBearerToken,
                JwtBearerToken.GetExpiryInEpoch(bearerTokenExpiryDate),
                newRefreshToken,
                user.BiometricOn, 
                false);
        }
    }
}
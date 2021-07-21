using Airslip.Common.Contracts;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Security.Jwt;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Commands
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
            User user = await _userService.Get(request.UserId);
            if (user.RefreshTokens == null ||
                !user.RefreshTokens.Contains(new RefreshToken(request.DeviceId, request.Token)))
                return new ResourceNotFound(nameof(RefreshToken),
                    "An incorrect refresh token has been used for this device");

            DateTime bearerTokenExpiryDate = JwtBearerToken.GetExpiry(_jwtSettings.ExpiresTime);

            string jwtBearerToken = JwtBearerToken.Generate(
                _jwtSettings.Key,
                _jwtSettings.Audience,
                _jwtSettings.Issuer,
                bearerTokenExpiryDate,
                request.UserId);

            string newRefreshToken = JwtBearerToken.GenerateRefreshToken();
            await _userService.UpdateRefreshToken(request.UserId, request.DeviceId, newRefreshToken);

            bool hasAddedInstitution = user.Institutions.Count > 0;

            return new AuthenticatedUserResponse(
                jwtBearerToken,
                JwtBearerToken.GetExpiryInEpoch(bearerTokenExpiryDate),
                newRefreshToken,
                hasAddedInstitution,
                new UserSettingsResponse(user.Settings.HasFaceId, false));
        }
    }
}
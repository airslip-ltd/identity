using Airslip.Common.Contracts;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Security.Jwt;
using MediatR;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Commands
{
    public class GenerateJwtBearerTokenCommandHandler : IRequestHandler<LoginUserCommand, IResponse>
    {
        private readonly IUserService _userService;
        private readonly IUserManagerService _userManagerService;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger _logger;

        public GenerateJwtBearerTokenCommandHandler(
            IUserService userService,
            IOptions<JwtSettings> jwtSettingsOptions,
            IUserManagerService userManagerService)
        {
            _userService = userService;
            _userManagerService = userManagerService;
            _jwtSettings = jwtSettingsOptions.Value;
            _logger = Log.Logger;
        }

        public async Task<IResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            //string encryptedEmail = Cryptography.GenerateSHA256String(command.Email);

            bool canLogin = await _userManagerService.TryToLogin(request.Email, request.Password);

            if (!canLogin)
            {
                User? possibleUser = await _userService.GetByEmail(request.Email);
                return possibleUser == null
                    ? new NotFoundResponse(
                        nameof(request.Email),
                        request.Email,
                        "A user with this email doesn't exist")
                    : new ErrorResponse("INCORRECT_PASSWORD", "Password is incorrect");
            }

            User? user = await _userService.GetByEmail(request.Email);
            if (user == null)
                return new NotFoundResponse(nameof(request.Email), request.Email, "Unable to find user");

            _logger.Information("User {UserId} successfully logged in", user.Id);

            DateTime bearerTokenExpiryDate = JwtBearerToken.GetExpiry(_jwtSettings.ExpiresTime);

            string jwtBearerToken = JwtBearerToken.Generate(
                _jwtSettings.Key,
                _jwtSettings.Audience,
                _jwtSettings.Issuer,
                bearerTokenExpiryDate,
                user.Id);

            string refreshToken = JwtBearerToken.GenerateRefreshToken();

            await _userService.UpdateRefreshToken(user.Id, request.DeviceId, refreshToken);

            return new AuthenticatedUserResponse(
                jwtBearerToken,
                JwtBearerToken.GetExpiryInEpoch(bearerTokenExpiryDate),
                refreshToken,
                user.BiometricOn, 
                true);
        }
    }
}
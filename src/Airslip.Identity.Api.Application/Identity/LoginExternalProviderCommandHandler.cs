﻿using Airslip.Common.Contracts;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Security.Jwt;
using Airslip.Yapily.Client.Contracts;
using MediatR;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class LoginExternalProviderCommandHandler : IRequestHandler<LoginExternalProviderCommand, IResponse>
    {
        private readonly IUserService _userService;
        private readonly IYapilyClient _yapilyApis;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger _logger;
        private readonly IUserProfileService _userProfileService;

        public LoginExternalProviderCommandHandler(
            IUserService userService,
            IYapilyClient yapilyApis,
            IOptions<JwtSettings> jwtSettingsOptions,
            IUserProfileService userProfileService)
        {
            _userService = userService;
            _yapilyApis = yapilyApis;
            _userProfileService = userProfileService;
            _jwtSettings = jwtSettingsOptions.Value;
            _logger = Log.Logger;
        }

        public async Task<IResponse> Handle(LoginExternalProviderCommand request, CancellationToken cancellationToken)
        {
            _logger.ForContext(nameof(request.Email), request.Email);

            User? user = await _userService.GetByEmail(request.Email);
            bool isNewUser = user is null;
            if (user is null)
            {
                IYapilyResponse response =
                    await _yapilyApis.CreateUser(request.Email, request.ReferenceId, cancellationToken);

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

                        if (yapilyUser.IsInvalid())
                        {
                            await _yapilyApis.DeleteUser(yapilyUser.Uuid!, cancellationToken);
                            return new ResourceNotFound(nameof(User), "Unable to create with all the required fields");
                        }

                        string email = yapilyUser.ApplicationUserId!;
                        string userId = yapilyUser.Uuid!;
                        
                        await _userService.Create(
                            new User(
                                userId,
                                yapilyUser.ApplicationUuid!,
                                email,
                                yapilyUser.ReferenceId!));

                        await _userProfileService.Create(new UserProfile(userId, request.Email));

                        user = await _userService.Get(yapilyUser.Uuid!);

                        _logger.Information("User {UserId} successfully logged in with {ExternalProvider}",
                            user.Id,
                            request.Provider);

                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            DateTime bearerTokenExpiryDate = DateTime.Now.AddSeconds(_jwtSettings.ExpiresTime);

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
                isNewUser);
        }
    }
}
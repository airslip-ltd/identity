﻿using Airslip.Common.Contracts;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Security.Jwt;
using Airslip.Yapily.Client.Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IResponse>
    {
        private readonly IUserService _userService;
        private readonly IYapilyClient _yapilyApis;
        private readonly JwtSettings _jwtSettings;
        private readonly IUserManagerService _userManagerService;
        private readonly ILogger _logger;

        public RegisterUserCommandHandler(
            IUserService userService,
            IYapilyClient yapilyApis,
            IOptions<JwtSettings> jwtSettingsOptions,
            IUserManagerService userManagerService)
        {
            _userService = userService;
            _yapilyApis = yapilyApis;
            _jwtSettings = jwtSettingsOptions.Value;
            _userManagerService = userManagerService;
            _logger = Log.Logger;
        }

        public async Task<IResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            //string encryptedEmail = Cryptography.GenerateSHA256String(command.Email);

            ILogger logger = Log
                .ForContext(nameof(request.Email), request.Email);

            IYapilyResponse response =
                await _yapilyApis.CreateUser(request.Email, request.ReferenceId, cancellationToken);

            switch (response)
            {
                case YapilyApiResponseError apiError:
                    switch (apiError.Error.Code)
                    {
                        case (int)HttpStatusCode.Conflict:
                            return new ConflictResponse(nameof(request.Email), request.Email, "User already exists");
                        default:
                            logger.Fatal("UNHANDLED_YAPILY_ERROR. ErrorMessage : {ErrorMessage}",
                                apiError.Error.Message);
                            throw new InvalidOperationException();
                    }

                case YapilyUser yapilyUser:

                    if (yapilyUser.IsInvalid())
                    {
                        await _yapilyApis.DeleteUser(yapilyUser.Uuid!, cancellationToken);
                        return new ResourceNotFound(nameof(User), "Unable to create with all the required fields");
                    }

                    await _userService.Create(
                        new User(
                            yapilyUser.Uuid!,
                            yapilyUser.ApplicationUuid!,
                            yapilyUser.ApplicationUserId!,
                            yapilyUser.ReferenceId!));

                    IdentityResult result = await _userManagerService.Create(request.Email, request.Password);

                    if (result.Succeeded is false)
                        return result.Errors.First().Code switch
                        {
                            "DuplicateUserName" => new ConflictResponse(
                                nameof(request.Email),
                                request.Email,
                                "User already exists"),
                            _ => new ErrorResponse(result.Errors.First().Code,
                                result.Errors.First().Description)
                        };

                    User user = await _userService.Get(yapilyUser.Uuid!);

                    _logger.Information("User {UserId} successfully registered", user.Id);

                    DateTime bearerTokenExpiryDate = JwtBearerToken.GetExpiry(_jwtSettings.ExpiresTime);

                    string jwtBearerToken = JwtBearerToken.Generate(
                        _jwtSettings.Key,
                        _jwtSettings.Audience,
                        _jwtSettings.Issuer,
                        bearerTokenExpiryDate,
                        user.Id);

                    bool hasAddedInstitution = user.Institutions.Count > 0;

                    string refreshToken = JwtBearerToken.GenerateRefreshToken();

                    await _userService.UpdateRefreshToken(user.Id, request.DeviceId, refreshToken);

                    return new AuthenticatedUserResponse(
                        jwtBearerToken,
                        JwtBearerToken.GetExpiryInEpoch(bearerTokenExpiryDate),
                        refreshToken,
                        hasAddedInstitution,
                        new UserSettingsResponse(user.Settings.HasFaceId, true));

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}

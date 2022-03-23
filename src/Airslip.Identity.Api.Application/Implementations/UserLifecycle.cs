using Airslip.Common.Auth.Data;
using Airslip.Common.Auth.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Data;
using Airslip.Common.Repository.Types.Entities;
using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Common.Repository.Types.Models;
using Airslip.Common.Types.Interfaces;
using Airslip.Common.Types.Failures;
using Airslip.Common.Utilities.Extensions;
using Airslip.Identity.Api.Application.Identity;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using Airslip.Identity.Api.Contracts.Responses;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class UserLifecycle : IUserLifecycle
    {
        private readonly ITokenGenerationService<GenerateUserToken> _tokenGenerationService;
        private readonly ITokenDecodeService<UserToken> _tokenDecodeService;
        private readonly IRepository<User, UserModel> _repository;
        private readonly IUserManagerService _userManagerService;
        private readonly IModelMapper<UserModel> _mapper;
        private readonly IIdentityContext _context;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly ILogger _logger;
        private readonly UserToken _userToken;

        public UserLifecycle(
            ITokenGenerationService<GenerateUserToken> tokenGenerationService, 
            ITokenDecodeService<UserToken> tokenDecodeService,
            IRepository<User, UserModel> repository,
            IUserManagerService userManagerService,
            IModelMapper<UserModel> mapper, 
            IIdentityContext context,
            IEmailNotificationService emailNotificationService,
            ILogger logger)
        {
            _tokenGenerationService = tokenGenerationService;
            _tokenDecodeService = tokenDecodeService;
            _userToken = tokenDecodeService.GetCurrentToken();
            _repository = repository;
            _userManagerService = userManagerService;
            _mapper = mapper;
            _context = context;
            _emailNotificationService = emailNotificationService;
            _logger = logger;
        }
        
        private async Task<IResponse> GenerateRefreshToken(string userId, string deviceId, string currentToken)
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
                deviceId);
        }

        public async Task<IResponse> GenerateRefreshToken(string deviceId, string currentToken)
        {
            // Attempt to get token from header
            Tuple<UserToken, ICollection<Claim>> result = _tokenDecodeService.DecodeTokenFromHeader(
                AirslipSchemeOptions.JwtBearerHeaderField, AirslipSchemeOptions.JwtBearerScheme);

            return await GenerateRefreshToken(result.Item1.UserId, deviceId, currentToken);
        }

        public async Task<IResponse> GenerateUserResponse(User user, 
            bool isNewUser, 
            string deviceId = "")
        {

            string[] applicationRoles = await _userManagerService.GetRoles(user.Email);
            
            GenerateUserToken generateUserToken = new(user.EntityId ?? string.Empty, 
                user.AirslipUserType,
                user.Id, 
                string.Empty,
                user.UserRole,
                applicationRoles);

            NewToken newToken = _tokenGenerationService.GenerateNewToken(generateUserToken);
            string newRefreshToken = JwtBearerToken.GenerateRefreshToken();
            
            await _context.UpdateOrReplaceRefreshToken(user.Id, deviceId, newRefreshToken);

            return new AuthenticatedUserResponse(
                newToken.TokenValue,
                newToken.TokenExpiry?.ToUnixTimeMilliseconds() ?? 0,
                newRefreshToken,
                isNewUser,
                _mapper.Create(user));
        }

        public async Task<IResponse> Register(RegisterUserCommand model, CancellationToken cancellationToken,
            string? userId = null)
        {
            IResponse addUserResult = await Add(model, cancellationToken, userId);

            if (addUserResult is not SuccessfulActionResultModel<UserModel> success)
                return addUserResult;

            string id = success.CurrentVersion!.Id!;
            User user = (await _context.GetEntity<User>(id))!;

            return await GenerateUserResponse(user, true, model.DeviceId);
        }

        public async Task<IResponse> Add(RegisterUserCommand model, CancellationToken cancellationToken, string? userId = null)
        {
            IdentityResult result;
            if (!string.IsNullOrEmpty(model.Password))
            {
                result = await _userManagerService
                    .Create(model.Email, model.Password);
            }
            else
            {
                result = await _userManagerService
                    .Create(model.Email);

            }
            
            if (result.Succeeded is false)
                return result.Errors.First().Code switch
                {
                    "DuplicateUserName" => new ConflictResponse(
                        nameof(model.Email),
                        model.Email,
                        "User already exists"),
                    "PasswordRequiresUpper" => new IncorrectPasswordResponse("Passwords must have at least one non alphanumeric character."),
                    _ => new ErrorResponse(
                        result.Errors.First().Code,
                        result.Errors.First().Description)
                };

            string userRole = model.UserRole ?? UserRoles.User;
            
            await _userManagerService
                .SetRole(model.Email, userRole);
            
            User? user = await _context.GetByEmail(model.Email);

            if (user is null) {
                User newUser = new(model.Email, model.FirstName, model.LastName, userRole)
                {
                    DisplayName = model.DisplayName ?? $"{model.FirstName} {model.LastName}".Trim(),
                    EntityId = _userToken.IsAuthenticated ?? false ? 
                        _userToken.EntityId : 
                        model.EntityId,
                    AirslipUserType = _userToken.IsAuthenticated ?? false ? 
                        _userToken.AirslipUserType : 
                        model.AirslipUserType,
                    UserRole = userRole,
                    AuditInformation = new BasicAuditInformation
                    {
                        DateCreated = DateTime.UtcNow,
                        CreatedByUserId = userId ?? _userToken.UserId
                    }
                };
                user = await _context.AddEntity(newUser);

                await SetRole(user.Id, userRole);
            } else {
                user.ChangeFromUnregisteredToStandard();
                await _context.UpdateEntity(user);
            }

            await _emailNotificationService.SendNewUserEmail(user.Email, user.FirstName ?? user.Email, "auth/create");

            return await _repository.Get(user.Id);
        }

        public async Task<IResponse> SetRole(string id, string roleName)
        {
            if (id == _userToken.UserId && roleName != _userToken.UserRole)
            {
                return new ErrorResponse(ErrorCodes.ValidationFailed,
                    "You cannot change your own role, please ask a system administrator for help");
            }
            
            IResponse result = await _userManagerService
                .SetRole(id, roleName);

            return result;
        }
    }
}
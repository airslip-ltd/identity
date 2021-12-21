using Airslip.Common.Auth.Data;
using Airslip.Common.Auth.Extensions;
using Airslip.Common.Auth.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Data;
using Airslip.Common.Repository.Entities;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Models;
using Airslip.Common.Types.Interfaces;
using Airslip.Common.Types.Failures;
using Airslip.Common.Utilities.Extensions;
using Airslip.Identity.Api.Application.Identity;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Yapily.Client.Contracts;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class UserService : IUserService
    {
        private readonly ITokenGenerationService<GenerateUserToken> _tokenGenerationService;
        private readonly ITokenDecodeService<UserToken> _tokenDecodeService;
        private readonly IRepository<User, UserModel> _repository;
        private readonly IUserManagerService _userManagerService;
        private readonly IModelMapper<UserModel> _mapper;
        private readonly IIdentityContext _context;
        private readonly IYapilyClient _yapilyApis;
        private readonly ILogger _logger;
        private readonly UserToken _userToken;

        public UserService(
            ITokenGenerationService<GenerateUserToken> tokenGenerationService, 
            ITokenDecodeService<UserToken> tokenDecodeService,
            IRepository<User, UserModel> repository,
            IUserManagerService userManagerService,
            IModelMapper<UserModel> mapper, 
            IIdentityContext context,
            IYapilyClient yapilyApis,
            ILogger logger)
        {
            _tokenGenerationService = tokenGenerationService;
            _tokenDecodeService = tokenDecodeService;
            _userToken = tokenDecodeService.GetCurrentToken();
            _repository = repository;
            _userManagerService = userManagerService;
            _mapper = mapper;
            _context = context;
            _yapilyApis = yapilyApis;
            _logger = logger;
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

        public async Task<IResponse> GenerateRefreshToken(string deviceId, string currentToken)
        {
            // Attempt to get token from header
            Tuple<UserToken, ICollection<Claim>> result = _tokenDecodeService.DecodeTokenFromHeader(
                AirslipSchemeOptions.JwtBearerHeaderField, AirslipSchemeOptions.JwtBearerScheme);

            return await GenerateRefreshToken(result.Item1.UserId, deviceId, currentToken);
        }

        public async Task<IResponse> GenerateUserResponse(User user, 
            bool isNewUser,
            string? yapilyUserId = null, 
            string deviceId = "")
        {
            yapilyUserId ??= user.GetOpenBankingProviderId("Yapily");

            string[] applicationRoles = await _userManagerService.GetRoles(user.Email);
            
            GenerateUserToken generateUserToken = new(user.EntityId ?? string.Empty, 
                user.AirslipUserType,
                user.Id, 
                yapilyUserId ?? string.Empty,
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
            
            IYapilyResponse response =
                await _yapilyApis.CreateUser(id, model.ReferenceId, cancellationToken);

            switch (response)
            {
                case YapilyApiResponseError apiError:
                    switch (apiError.Error.Code)
                    {
                        case (int) HttpStatusCode.Conflict:
                            return new ConflictResponse(nameof(model.Email), model.Email, "User already exists");
                        default:
                            _logger.Fatal("UNHANDLED_YAPILY_ERROR. ErrorMessage : {ErrorMessage}",
                                apiError.Error.Message);
                            throw new InvalidOperationException();
                    }

                case YapilyUser yapilyUser:
                    string yapilyUserId = yapilyUser.Uuid;
                    string yapilyApplicationId = yapilyUser.ApplicationUuid;
                    string yapilyReferenceId = yapilyUser.ReferenceId;

                    if (yapilyUser.IsInvalid())
                    {
                        await _yapilyApis.DeleteUser(yapilyUserId, cancellationToken);
                        return new ResourceNotFound(nameof(User), "Unable to create with all the required fields");
                    }
                    
                    user.AddOpenBankingProvider( new OpenBankingProvider("Yapily", yapilyUserId, yapilyApplicationId, yapilyReferenceId));
                    
                    await _context.UpdateEntity(user);
                    
                    _logger.Information("User {UserId} successfully registered with email {Email} at {NowDate}", user.Id, model.Email, DateTimeOffset.UtcNow);

                    return await GenerateUserResponse(user, true, yapilyUserId, model.DeviceId);

                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<IResponse> Add(RegisterUserCommand model, CancellationToken cancellationToken, string? userId = null)
        {
            IdentityResult result = await _userManagerService
                .Create(model.Email, model.Password);
            
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
                    DisplayName = $"{model.FirstName} {model.LastName}".Trim(),
                    EntityId = _userToken.EntityId.IsNullOrWhitespace() ? null : _userToken.EntityId,
                    AirslipUserType = _userToken.AirslipUserType,
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

            return await _repository.Get(user.Id);
        }

        public async Task<IResponse> Update(string id, UserModel model, string? userId = null)
        {
            RepositoryActionResultModel<UserModel> repositoryActionResult = await _repository.Update(id, model, userId);

            if (repositoryActionResult is not SuccessfulActionResultModel<UserModel> success)
                return repositoryActionResult;
            
            await _userManagerService
                .ChangeEmail(success.PreviousVersion!.Email, success.CurrentVersion!.Email);
                
            await SetRole(success.CurrentVersion!.Id ?? throw new ArgumentException(), 
                success.CurrentVersion.UserRole ?? UserRoles.User);

            return repositoryActionResult;
        }

        public async Task<IResponse> Delete(string id, string? userId = null)
        {
            if (id == _userToken.UserId)
            {
                return new ErrorResponse(ErrorCodes.ValidationFailed,
                    "You cannot delete your own user account, please ask a system administrator for help");
            }
            RepositoryActionResultModel<UserModel> repositoryActionResult = await _repository.Delete(id, userId);
            
            if (repositoryActionResult is SuccessfulActionResultModel<UserModel> success)
            {
                await _userManagerService.Delete(success.PreviousVersion!.Email);
            }

            return repositoryActionResult;
        }

        public async Task<IResponse> Get(string id)
        {
            return await _repository.Get(id);
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
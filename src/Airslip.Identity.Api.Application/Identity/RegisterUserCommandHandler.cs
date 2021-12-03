using Airslip.Common.Auth.Data;
using Airslip.Common.Types.Interfaces;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts;
using Airslip.Identity.Api.Contracts.Data;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Yapily.Client.Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IResponse>
    {
        private readonly IUserService _userService;
        private readonly IIdentityContext _context;
        private readonly IYapilyClient _yapilyApis;
        private readonly IUserManagerService _userManagerService;
        private readonly ILogger _logger;

        public RegisterUserCommandHandler(
            IUserService userService,
            IIdentityContext context,
            IYapilyClient yapilyApis,
            IUserManagerService userManagerService)
        {
            _userService = userService;
            _context = context;
            _yapilyApis = yapilyApis;
            _userManagerService = userManagerService;
            _logger = Log.Logger;
        }

        public async Task<IResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            IdentityResult result = await _userManagerService
                .Create(request.Email, request.Password);
            
            if (result.Succeeded is false)
                return result.Errors.First().Code switch
                {
                    "DuplicateUserName" => new ConflictResponse(
                        nameof(request.Email),
                        request.Email,
                        "User already exists"),
                    "PasswordRequiresUpper" => new IncorrectPasswordResponse("Passwords must have at least one non alphanumeric character."),
                    _ => new ErrorResponse(
                        result.Errors.First().Code,
                        result.Errors.First().Description)
                };

            string userRole = request.UserRole ?? UserRoles.User;
            
            await _userManagerService
                .SetRole(request.Email, userRole);
            
            User? user = await _context.GetByEmail(request.Email);

            if (user is null) {
                User newUser = new(request.Email, request.FirstName, request.LastName, userRole);
            
                user = await _context.AddEntity(newUser);
            } else {
                user.ChangeFromUnregisteredToStandard();
                await _context.UpdateEntity(user);
            }

            user.EntityId = request.EntityId;
            user.AirslipUserType = request.AirslipUserType;
            user.UserRole = userRole;
            
            IYapilyResponse response =
                await _yapilyApis.CreateUser(user.Id, request.ReferenceId, cancellationToken);

            switch (response)
            {
                case YapilyApiResponseError apiError:
                    switch (apiError.Error.Code)
                    {
                        case (int) HttpStatusCode.Conflict:
                            return new ConflictResponse(nameof(request.Email), request.Email, "User already exists");
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
                    
                    _logger.Information("User {UserId} successfully registered with email {Email} at {NowDate}", user.Id, request.Email, DateTimeOffset.UtcNow);

                    return await _userService
                        .GenerateUserResponse(user, true, yapilyUserId, request.DeviceId);

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
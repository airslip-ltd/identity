using Airslip.Common.Auth.AspNetCore.Implementations;
using Airslip.Common.Auth.Data;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Types.Interfaces;
using Airslip.Common.Types;
using Airslip.Common.Types.Configuration;
using Airslip.Common.Types.Failures;
using Airslip.Common.Utilities;
using Airslip.Identity.Api.Application.Identity;
using Airslip.Identity.Api.Contracts;
using Airslip.Identity.Api.Contracts.Requests;
using Airslip.Identity.Api.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Alpha2CountryCodes = Airslip.Common.Types.Enums.Alpha2CountryCodes;

namespace Airslip.Identity.Api.Controller
{
    [ApiController]
    [ApiVersion(ApiConstants.VersionOne)]
    [Route("v{version:apiVersion}/identity")]
    [Produces(Json.MediaType)]
    public class IdentityController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly PublicApiSetting _bankTransactionSettings;

        public IdentityController(
            ITokenDecodeService<UserToken> tokenService,
            ILogger logger,
            IOptions<PublicApiSettings> publicApiOptions,
            IMediator mediator) : base(tokenService, publicApiOptions, logger)
        {
            _mediator = mediator;
            _bankTransactionSettings = publicApiOptions.Value.BankTransactions ?? throw new ArgumentException(
                "PublicApiSettings:BankTransactions section missing from appSettings",
                nameof(publicApiOptions));
        }

        [HttpPost("check")]
        [ProducesResponseType(typeof(AuthenticatedUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthenticatedUserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckUserExists(CheckUserRequest request)
        {
            CheckUserCommand checkUserCommand = new(
                request.Email);

            IResponse response = await _mediator.Send(checkUserCommand);

            return response switch
            {
                UserResponse r => Ok(r),
                _ => throw new NotSupportedException()
            };
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthenticatedUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthenticatedUserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> IdentityLogin(LoginRequest request)
        {
            LoginUserCommand loginUserCommand = new(
                request.Email,
                request.Password,
                request.DeviceId);

            IResponse result = await _mediator.Send(loginUserCommand);

            if (result is NotFoundResponse && request.CreateUserIfNotExists)
            {
                return await IdentityRegister(new RegisterRequest
                {
                    Email = request.Email,
                    Password = request.Password,
                    DeviceId = request.DeviceId,
                    EntityId = request.EntityId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserRole = UserRoles.User,
                    AirslipUserType = request.AirslipUserType
                });
            }

            return HandleResponse<AuthenticatedUserResponse>(result);
        }
        
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthenticatedUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthenticatedUserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> IdentityRegister(RegisterRequest request)
        {
            RegisterUserCommand registerUserCommand = new(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password,
                request.DeviceId,
                request.UserRole)
            {
                EntityId = request.EntityId,
                AirslipUserType = request.AirslipUserType
            };

            IResponse response = await _mediator.Send(registerUserCommand);

            return HandleResponse<AuthenticatedUserResponse>(response);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticatedUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GenerateRefreshToken(RefreshTokenRequest request)
        {
            GenerateRefreshTokenCommand command = new(
                request.DeviceId,
                request.RefreshToken);

            IResponse response = await _mediator.Send(command);
            
            return HandleResponse<AuthenticatedUserResponse>(response);
        }

        [HttpGet("logout")]
        public IActionResult IdentityLogout()
        {
            return Ok();
        }

        [HttpPost("recovery")]
        [ProducesResponseType(typeof(ForgotPasswordResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            ForgotPasswordCommand command = new(
                "auth/forgot",
                forgotPasswordRequest.Email
            );

            IResponse response = await _mediator.Send(command);

            return HandleResponse<Success>(response);
        }

        [HttpPost("password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            ResetPasswordCommand command = new(
                resetPasswordRequest.Password,
                resetPasswordRequest.ConfirmPassword,
                resetPasswordRequest.Email,
                resetPasswordRequest.Token);

            IResponse response = await _mediator.Send(command);

            return HandleResponse<Success>(response);
        }
    }
}
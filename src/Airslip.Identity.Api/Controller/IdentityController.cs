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
    // ReSharper disable once RouteTemplates.RouteParameterConstraintNotResolved
    [Route("v{version:apiVersion}/identity")]
    [Produces(Json.MediaType)]
    public class IdentityController : ApiResponse
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

            IResponse getUserByEmailResponse = await _mediator.Send(loginUserCommand);
            switch (getUserByEmailResponse)
            {
                case AuthenticatedUserResponse response:
                    return Ok(response.AddHateoasLinks(_publicApiSettings.Base.BaseUri,
                        _bankTransactionSettings.BaseUri, false,
                        Alpha2CountryCodes.GB.ToString()));
                case IncorrectPasswordResponse incorrectPasswordResponse:
                    return Forbidden(incorrectPasswordResponse);
                case NotFoundResponse response:
                {
                    if (request.CreateUserIfNotExists)
                    {
                        return await IdentityRegister(new RegisterRequest()
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

                    return NotFound(response);
                }
                case ErrorResponse response:
                    return BadRequest(response);
                case IFail response:
                    return BadRequest(response);
                default:
                    throw new NotSupportedException();
            }
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

            IResponse createUserResponse = await _mediator.Send(registerUserCommand);

            return createUserResponse switch
            {
                AuthenticatedUserResponse response => Created(response.AddHateoasLinks(
                    _publicApiSettings.Base.BaseUri,
                    _bankTransactionSettings.BaseUri,
                    true,
                    Alpha2CountryCodes.GB.ToString())),
                ConflictResponse response => Conflict(response),
                NotFoundResponse r => NotFound(r),
                ErrorResponse response => BadRequest(response),
                IFail response => BadRequest(response),
                _ => throw new NotSupportedException()
            };
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
            return response switch
            {
                AuthenticatedUserResponse r => Ok(r.AddHateoasLinks(
                    _publicApiSettings.Base.BaseUri,
                    _bankTransactionSettings.BaseUri,
                    false,
                    Alpha2CountryCodes.GB.ToString())),
                NotFoundResponse r => NotFound(r),
                ErrorResponse r => BadRequest(r),
                IFail r => BadRequest(r),
                _ => throw new NotSupportedException()
            };
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            AuthenticationProperties properties = new() { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            ClaimsIdentity? claimIdentity = result.Principal?.Identities.FirstOrDefault();

            if (claimIdentity == null)
                return BadRequest(new ExternalLoginFailed(GoogleDefaults.AuthenticationScheme));

            ExternalLoginResponse externalLoginResponse = GetExternalLoginResponse(
                claimIdentity,
                result.Properties?.ExpiresUtc);

            LoginExternalProviderCommand loginExternalProviderCommand = new(
                externalLoginResponse.Email,
                GoogleDefaults.AuthenticationScheme,
                externalLoginResponse.DeviceId,
                null,
                null);

            IResponse loginExternalProviderResponse = await _mediator.Send(loginExternalProviderCommand);

            return loginExternalProviderResponse switch
            {
                AuthenticatedUserResponse response => Ok(response.AddHateoasLinks(
                    _publicApiSettings.Base.BaseUri,
                    _bankTransactionSettings.BaseUri,
                    response.IsNewUser,
                    Alpha2CountryCodes.GB.ToString())),
                ErrorResponse response => BadRequest(response),
                IFail response => BadRequest(response),
                _ => throw new NotSupportedException()
            };
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleSignin(GoogleSigninRequest request)
        {
            LoginExternalProviderCommand loginExternalProviderCommand = new(
                request.Email,
                GoogleDefaults.AuthenticationScheme,
                request.DeviceId,
                null, null);

            IResponse loginExternalProviderResponse = await _mediator.Send(loginExternalProviderCommand);

            return loginExternalProviderResponse switch
            {
                AuthenticatedUserResponse response => Ok(response.AddHateoasLinks(
                    _publicApiSettings.Base.BaseUri,
                    _bankTransactionSettings.BaseUri,
                    response.IsNewUser,
                    Alpha2CountryCodes.GB.ToString())),
                ErrorResponse response => BadRequest(response),
                IFail response => BadRequest(response),
                _ => throw new NotSupportedException()
            };
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
                "v1/identity/password",
                forgotPasswordRequest.Email
            );

            IResponse response = await _mediator.Send(command);

            return response switch
            {
                ISuccess => NoContent(),
                NotFoundResponse r => NotFound(r),
                _ => BadRequest(response)
            };
        }

        [HttpGet("password")]
        public IActionResult ResetPassword([FromQuery] string token, [FromQuery] string email)
        {
            return Ok(new
            {
                Token = token.Replace(" ",
                    "+"), // Need to find out if there is a better as the query string is losing the + character
                Email = email
            });
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

            return response switch
            {
                ISuccess => Ok(response),
                NotFoundResponse r => NotFound(r),
                _ => BadRequest(response)
            };
        }

        private static ExternalLoginResponse GetExternalLoginResponse(
            ClaimsIdentity claimIdentity,
            DateTimeOffset? expires)
        {
            var claims = claimIdentity.Claims.Select(claim => new
            {
                claim.Type,
                claim.Value
            }).ToList();

            var firstNameClaim = claims.FirstOrDefault(x => x.Type.Contains("givenname"))!;
            var surnameClaim = claims.FirstOrDefault(x => x.Type.Contains("surname"))!;
            var emailAddressClaim = claims.FirstOrDefault(x => x.Type.Contains("emailaddress"))!;
            var nameIdentifier = claims.FirstOrDefault(x => x.Type.Contains("nameidentifier"))!;

            return new ExternalLoginResponse(
                firstNameClaim.Value,
                surnameClaim.Value,
                emailAddressClaim.Value,
                nameIdentifier.Value,
                expires);
        }
    }
}
using Airslip.Common.Contracts;
using Airslip.Common.Types.Enums;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Application.Commands;
using Airslip.Identity.Api.Auth;
using Airslip.Identity.Api.Contracts.Requests;
using Airslip.Identity.Api.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Controller
{
    [ApiController]
    [AllowAnonymous]
    [ApiVersion(ApiConstants.VersionOne)]
    // ReSharper disable once RouteTemplates.RouteParameterConstraintNotResolved
    [Route("v{version:apiVersion}/identity")]
    [Produces(ApiConstants.JsonMediaType)]
    public class IdentityController : ApiResponse
    {
        private readonly IMediator _mediator;

        public IdentityController(
            Token token,
            IOptions<PublicApiSettings> publicApiOptions,
            IMediator mediator) : base(token, publicApiOptions)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthenticatedUserResponse), StatusCodes.Status200OK)]
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
                    return Ok(response.AddHateoasLinks(_publicApiSettings.BaseUri,
                        _publicApiSettings.BankTransactionsUri, response.HasAddedInstitution,
                        Alpha2CountryCodes.GB.ToString()));
                case NotFoundResponse _:
                {
                    RegisterUserCommand registerUserCommand = new(
                        request.Email,
                        request.Password,
                        request.DeviceId);

                    IResponse createUserResponse = await _mediator.Send(registerUserCommand);

                    return createUserResponse switch
                    {
                        AuthenticatedUserResponse response => Ok(response.AddHateoasLinks(
                            _publicApiSettings.BaseUri,
                            _publicApiSettings.BankTransactionsUri,
                            response.HasAddedInstitution,
                            Alpha2CountryCodes.GB.ToString())),
                        ConflictResponse response => Conflict(response),
                        ErrorResponse response => BadRequest(response),
                        IFail response => BadRequest(response),
                        _ => throw new NotSupportedException()
                    };
                }
                case ErrorResponse response:
                    return BadRequest(response);
                case IFail response:
                    return BadRequest(response);
                default:
                    throw new NotSupportedException();
            }
        }

        [Authorize]
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthenticatedUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GenerateRefreshToken(RefreshTokenRequest request)
        {
            GenerateRefreshTokenCommand command = new(
                Token.UserId,
                request.DeviceId,
                request.RefreshToken);

            IResponse response = await _mediator.Send(command);

            return response switch
            {
                AuthenticatedUserResponse r => Ok(r.AddHateoasLinks(
                    _publicApiSettings.BaseUri,
                    _publicApiSettings.BankTransactionsUri,
                    r.HasAddedInstitution,
                    Alpha2CountryCodes.GB.ToString())),
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
                GoogleDefaults.AuthenticationScheme
            );

            IResponse loginExternalProviderResponse = await _mediator.Send(loginExternalProviderCommand);

            return loginExternalProviderResponse switch
            {
                AuthenticatedUserResponse response => Ok(response.AddHateoasLinks(
                    _publicApiSettings.BaseUri,
                    _publicApiSettings.BankTransactionsUri,
                    response.HasAddedInstitution,
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
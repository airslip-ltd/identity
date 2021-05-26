using Airslip.Common.Contracts;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Auth;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;

namespace Airslip.Identity.Api
{
    public class ApiResponse : ControllerBase
    {
        protected readonly PublicApiSettings _publicApiSettings;
        protected readonly Token Token;

        public ApiResponse(Token token, IOptions<PublicApiSettings> publicApiOptions)
        {
            Token = token;
            _publicApiSettings = publicApiOptions.Value;
        }

        protected IActionResult Ok(IResponse response)
        {
            return response is ISuccess
                ? new ObjectResult(response) {StatusCode = (int) HttpStatusCode.OK}
                : BadRequest(response);
        }

        protected IActionResult Created(IResponse response)
        {
            return response switch
            {
                ISuccess _ => new ObjectResult(response) {StatusCode = (int) HttpStatusCode.Created},
                _ => BadRequest(response)
            };
        }

        protected IActionResult Conflict(IResponse response)
        {
            return response is ConflictResponse
                ? new ObjectResult(response) {StatusCode = StatusCodes.Status409Conflict}
                : BadRequest(response);
        }

        protected IActionResult Unauthorised(IResponse response)
        {
            return response is UnauthorisedResponse
                ? new ObjectResult(response) {StatusCode = StatusCodes.Status401Unauthorized}
                : BadRequest(response);
        }

        protected IActionResult NotFound(IResponse response)
        {
            return response is NotFoundResponse
                ? new ObjectResult(response) {StatusCode = StatusCodes.Status404NotFound}
                : BadRequest(response);
        }

        protected IActionResult Accepted(IResponse response, Uri location)
        {
            return response is ISuccess
                ? Accepted(location, null)
                : BadRequest(response);
        }

        protected IActionResult NoContent(IResponse response)
        {
            return response is ISuccess
                ? new ObjectResult(null) {StatusCode = (int) HttpStatusCode.NoContent}
                : BadRequest(response);
        }

        protected IActionResult MethodNotAllowed(IResponse response)
        {
            return new ObjectResult(null)
                {StatusCode = (int) HttpStatusCode.MethodNotAllowed};
        }

        protected IActionResult BadRequest(IResponse failure)
        {
            return failure switch
            {
                ErrorResponse response => new BadRequestObjectResult(new ApiErrorResponse(Token, response)),
                ErrorResponses response => new BadRequestObjectResult(new ApiErrorResponse(Token, response.Errors)),
                IFail response => new BadRequestObjectResult(
                    new ApiErrorResponse(Token, new ErrorResponse(response.ErrorCode))),
                _ => throw new ArgumentException("Unknown response type.", nameof(failure))
            };
        }

        // protected async Task AddClaims(string userId)
        // {
        //     ClaimsIdentity identity = new(IdentityConstants.ApplicationScheme);
        //     identity.AddClaim(new Claim("userid", userId));
        //
        //     await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));
        // }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public class ApiErrorResponse
        {
            public ApiErrorResponse(Token token, ErrorResponse error)
                : this(token, new[] {error})
            {
            }

            public ApiErrorResponse(Token token, IEnumerable<ErrorResponse> errors)
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                CorrelationId = string.IsNullOrWhiteSpace(token.ThirdPartyCorrelationId)
                    ? token.CorrelationId
                    : token.ThirdPartyCorrelationId;
                Errors = errors;
            }

            public long Timestamp { get; }
            public string CorrelationId { get; }
            public IEnumerable<ErrorResponse> Errors { get; }
        }
    }
}
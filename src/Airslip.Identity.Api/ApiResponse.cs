using Airslip.Common.Contracts;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Auth;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Airslip.Identity.Api
{
    public class ApiResponse : ControllerBase
    {
        protected readonly PublicApiSettings _publicApiSettings;
        protected readonly Token Token;
        protected readonly ILogger _logger;

        public ApiResponse(Token token, IOptions<PublicApiSettings> publicApiOptions, ILogger logger)
        {
            Token = token;
            _publicApiSettings = publicApiOptions.Value;
            _logger = logger;
        }

        protected IActionResult Ok(IResponse response)
        {
            return response is ISuccess
                ? new ObjectResult(response) { StatusCode = (int) HttpStatusCode.OK }
                : BadRequest(response);
        }

        protected IActionResult Created(IResponse response)
        {
            return response switch
            {
                ISuccess _ => new ObjectResult(response) { StatusCode = (int) HttpStatusCode.Created },
                _ => BadRequest(response)
            };
        }

        protected IActionResult Conflict(IResponse response)
        {
            if (response is not ConflictResponse conflictResponse)
                return BadRequest(response);

            _logger.Warning("Conflict error: {ErrorMessage}", conflictResponse.Message);
            return new ObjectResult(response) { StatusCode = StatusCodes.Status409Conflict };
        }

        protected IActionResult Unauthorised(IResponse response)
        {
            switch (response)
            {
                case UnauthorisedResponse unauthorisedResponse:
                    _logger.Error("Unauthorised error: {ErrorMessage}", unauthorisedResponse.Message);
                    return new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
                default:
                    return BadRequest(response);
            }
        }

        protected IActionResult NotFound(IResponse response)
        {
            return response is NotFoundResponse
                ? new ObjectResult(response) { StatusCode = StatusCodes.Status404NotFound }
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
                ? new ObjectResult(null) { StatusCode = (int) HttpStatusCode.NoContent }
                : BadRequest(response);
        }

        protected IActionResult BadRequest(IResponse failure)
        {
            switch (failure)
            {
                case ErrorResponse response:
                    _logger.Error("Bad request error: {ErrorMessage}", response.Message);
                    return new BadRequestObjectResult(new ApiErrorResponse(Token, response));
                case ErrorResponses response:
                    _logger.Error("Bad request errors: {ErrorMessages}",
                        string.Join(",", response.Errors.Select(errorResponse => errorResponse.Message)));
                    return new BadRequestObjectResult(new ApiErrorResponse(Token, response.Errors));
                case IFail response:
                    _logger.Error("Fail errors: {ErrorMessages}", response.ErrorCode);
                    return new BadRequestObjectResult(
                        new ApiErrorResponse(Token, new ErrorResponse(response.ErrorCode)));
                default:
                    throw new ArgumentException("Unknown response type.", nameof(failure));
            }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public class ApiErrorResponse
        {
            public ApiErrorResponse(Token token, ErrorResponse error)
                : this(token, new[] { error })
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
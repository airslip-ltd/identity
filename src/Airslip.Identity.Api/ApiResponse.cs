// using Airslip.Common.Auth.Interfaces;
// using Airslip.Common.Auth.Models;
// using Airslip.Common.Types.Interfaces;
// using Airslip.Common.Repository.Types.Enums;
// using Airslip.Common.Repository.Types.Interfaces;
// using Airslip.Common.Repository.Types.Models;
// using Airslip.Common.Types.Configuration;
// using Airslip.Common.Types.Failures;
// using Airslip.Identity.Api.Contracts;
// using JetBrains.Annotations;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Options;
// using Serilog;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net;
//
// namespace Airslip.Identity.Api
// {
//     public class ApiResponse : ControllerBase
//     {
//         protected readonly PublicApiSettings _publicApiSettings;
//         protected readonly UserToken Token;
//         protected readonly ILogger _logger;
//
//         public ApiResponse(ITokenDecodeService<UserToken> tokenService, IOptions<PublicApiSettings> publicApiOptions, ILogger logger)
//         {
//             Token = tokenService.GetCurrentToken();
//             _publicApiSettings = publicApiOptions.Value;
//             _logger = logger;
//         }
//
//         protected IActionResult Ok(IResponse response)
//         {
//             return response is ISuccess
//                 ? new ObjectResult(response) { StatusCode = (int) HttpStatusCode.OK }
//                 : BadRequest(response);
//         }
//
//         protected IActionResult Created(IResponse response)
//         {
//             return response switch
//             {
//                 ISuccess _ => new ObjectResult(response) { StatusCode = (int) HttpStatusCode.Created },
//                 _ => BadRequest(response)
//             };
//         }
//
//         protected IActionResult Conflict(IResponse response)
//         {
//             if (response is not ConflictResponse conflictResponse)
//                 return BadRequest(response);
//
//             _logger.Warning("Conflict error: {ErrorMessage}", conflictResponse.Message);
//             return new ObjectResult(new ApiErrorResponse(Token, new[] { conflictResponse })) { StatusCode = StatusCodes.Status409Conflict };
//         }
//
//         protected IActionResult NotFound(IResponse response)
//         {
//             return response is NotFoundResponse errorResponse
//                 ? new ObjectResult(new ApiErrorResponse(Token, new[] { errorResponse })) { StatusCode = StatusCodes.Status404NotFound }
//                 : BadRequest(response);
//         }
//         
//         protected IActionResult Forbidden(IResponse response)
//         {
//             return response is IncorrectPasswordResponse errorResponse
//                 ? new ObjectResult(new ApiErrorResponse(Token, new[] { errorResponse })) { StatusCode = StatusCodes.Status403Forbidden }
//                 : BadRequest(response);
//         }
//
//         protected IActionResult BadRequest(IResponse failure)
//         {
//             switch (failure)
//             {
//                 case ErrorResponse response:
//                     _logger.Error("Bad request error: {ErrorMessage}", response.Message);
//                     return new BadRequestObjectResult(new ApiErrorResponse(Token, new[] { response }));
//                 case ErrorResponses response:
//                     _logger.Error("Bad request errors: {ErrorMessages}",
//                         string.Join(",", response.Errors.Select(errorResponse => errorResponse.Message)));
//                     return new BadRequestObjectResult(new ApiErrorResponse(Token, response.Errors));
//                 case IFail response:
//                     _logger.Error("Fail errors: {ErrorMessages}", response.ErrorCode);
//                     return new BadRequestObjectResult(
//                         new ApiErrorResponse(Token, new List<ErrorResponse> { new(response.ErrorCode) }));
//                 default:
//                     throw new ArgumentException("Unknown response type.", nameof(failure));
//             }
//         }
//
//         [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
//         public class ApiErrorResponse
//         {
//             public long Timestamp { get; }
//             public string CorrelationId { get; }
//             public IEnumerable<ErrorResponse> Errors { get; }
//
//             public ApiErrorResponse(UserToken token, IEnumerable<ErrorResponse> errors)
//             {
//                 Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
//                 CorrelationId = token.CorrelationId;
//                 Errors = errors;
//             }
//         }
//         
//         protected IActionResult RepositoryActionToResult<TModel>(RepositoryActionResultModel<TModel> theResult) 
//             where TModel : class, IModel
//         {
//             // Dependent on the return type, we will return either BadRequest or OK
//             if (theResult.ResultType == ResultType.NotFound)
//             {
//                 return NotFound(theResult);
//             } 
//             if (theResult.ResultType == ResultType.FailedValidation || theResult.ResultType == ResultType.FailedVerification)
//             {
//                 return BadRequest(theResult);
//             }
//
//             return Ok(theResult);
//         }
//     }
// }
// using Airslip.Common.Auth.Interfaces;
// using Airslip.Common.Auth.Models;
// using Airslip.Common.Repository.Types.Enums;
// using Airslip.Common.Repository.Types.Interfaces;
// using Airslip.Common.Repository.Types.Models;
// using Airslip.Common.Types.Configuration;
// using Airslip.Common.Types.Failures;
// using Airslip.Common.Types.Hateoas;
// using Airslip.Common.Types.Interfaces;
// using Airslip.Common.Utilities.Extensions;
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
// namespace Airslip.Identity.Api.Controller
// {
//     public abstract class ApiControllerBase : ControllerBase
//     {
//         protected readonly string BaseUri;
//         protected readonly UserToken Token;
//         protected readonly ILogger _logger;
//
//         public ApiControllerBase(ITokenDecodeService<UserToken> tokenDecodeService, 
//             IOptions<PublicApiSettings> publicApiOptions, ILogger logger)
//         {
//             Token = tokenDecodeService.GetCurrentToken();
//             BaseUri = publicApiOptions.Value.Base.ToBaseUri();
//             _logger = logger;
//         }
//         
//         protected IActionResult Ok<T>(T response) 
//             where T: class, IResponse
//         {
//             if (response is ILinkResourceBase @base)
//             {
//                 @base.AddHateoasLinks<T>(BaseUri);
//                 @base.AddChildHateoasLinks(@base, BaseUri);
//             }
//             
//             return response is ISuccess
//                 ? new ObjectResult(response)
//                 {
//                     StatusCode = (int) HttpStatusCode.OK
//                 }
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
//             return new ObjectResult(response) { StatusCode = StatusCodes.Status409Conflict };
//         }
//         
//         protected IActionResult NotFound(IResponse response)
//         {
//             return response switch
//             {
//                 ISuccess _ => new ObjectResult(response) { StatusCode = (int) HttpStatusCode.NotFound },
//                 _ => BadRequest(response)
//             };
//         }
//
//         protected IActionResult Unauthorised(IResponse response)
//         {
//             switch (response)
//             {
//                 case UnauthorisedResponse unauthorisedResponse:
//                     _logger.Error("Unauthorised error: {ErrorMessage}", unauthorisedResponse.Message);
//                     return new ObjectResult(response) { StatusCode = StatusCodes.Status401Unauthorized };
//                 default:
//                     return BadRequest(response);
//             }
//         }
//
//         protected IActionResult BadRequest(IResponse failure)
//         {
//             switch (failure)
//             {
//                 case ErrorResponse response:
//                     _logger.Error("Bad request error: {ErrorMessage}", response.Message);
//                     return new BadRequestObjectResult(new ApiErrorResponse(Token, response));
//                 case ErrorResponses response:
//                     _logger.Error("Bad request errors: {ErrorMessages}",
//                         string.Join(",", response.Errors.Select(errorResponse => errorResponse.Message)));
//                     return new BadRequestObjectResult(new ApiErrorResponse(Token, response.Errors));
//                 case IFail response:
//                     _logger.Error("Fail errors: {ErrorMessages}", response.ErrorCode);
//                     return new BadRequestObjectResult(
//                         new ApiErrorResponse(Token, new ErrorResponse(response.ErrorCode)));
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
//             public ApiErrorResponse(UserToken token, ErrorResponse error)
//                 : this(token, new[] { error })
//             {
//             }
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
//             return theResult.ResultType switch
//             {
//                 ResultType.NotFound => NotFound(theResult),
//                 ResultType.FailedValidation or ResultType.FailedVerification => BadRequest(theResult),
//                 _ => Ok(theResult)
//             };
//         }
//
//         protected IActionResult CommonResponseHandler<TExpectedType>(IResponse? response) 
//             where TExpectedType : class, IResponse
//         {
//             return response switch
//             {
//                 TExpectedType r => Ok(r),
//                 NotFoundResponse r => NotFound(r),
//                 IFail r => BadRequest(r),
//                 null => BadRequest(),
//                 _ => throw new InvalidOperationException()
//             };
//         }
//     }
// }
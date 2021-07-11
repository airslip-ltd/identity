using Airslip.Common.Contracts;
using Airslip.Common.Types.Failures;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            ObjectResult errorResponse;

            try
            {
                await _next(context);
                return;
            }
            catch (ValidationException exception)
            {
                string[] errorCodes = exception.Errors.Select(e => e.ErrorCode).ToArray();
                string compressed = string.Join(",", errorCodes);

                _logger.Warning("ErrorMessages {Compressed}", compressed);

                IFail[] errors = exception.Errors
                    .Select(error => new ErrorResponse(error.ErrorCode, error.ErrorMessage,
                        new RouteValueDictionary(error.CustomState)!))
                    .Cast<IFail>()
                    .ToArray();

                errorResponse = new ObjectResult(errors)
                {
                    StatusCode = (int) HttpStatusCode.BadRequest
                };
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "An unhandled error occurred");

                errorResponse = new ObjectResult(new
                {
                    timestamp = DateTimeOffset.UtcNow.ToString("s"),
                    errorMessage = exception.Message
                });
            }

            string result = JsonConvert.SerializeObject(errorResponse.Value, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            context.Response.ContentType = ApiConstants.JsonMediaType;
            context.Response.StatusCode = errorResponse.StatusCode ?? (int) HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(result);
        }
    }
}
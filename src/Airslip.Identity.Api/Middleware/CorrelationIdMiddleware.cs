using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            string correlationId = Guid.NewGuid().ToString();

            httpContext.Request.Headers.TryGetValue(ApiConstants.CorrelationIdName, out StringValues values);

            if (values.Count == 0)
                httpContext.Request.Headers.Add(ApiConstants.CorrelationIdName, correlationId);

            return _next(httpContext);
        }
    }
}
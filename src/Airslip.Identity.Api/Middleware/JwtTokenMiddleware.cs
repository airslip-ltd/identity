using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Middleware
{
    public class JwtTokenMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public JwtTokenMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                IIdentity? identity = httpContext.User.Identity;
                if (identity is { IsAuthenticated: true })
                {
                    AuthenticateResult authenticateResult = await httpContext.AuthenticateAsync("Bearer");
                    if (authenticateResult.Succeeded && authenticateResult.Principal != null)
                        httpContext.User = authenticateResult.Principal;
                }
                await _next(httpContext);
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "An unhandled authentication error occurred");
            }
        }
    }
}
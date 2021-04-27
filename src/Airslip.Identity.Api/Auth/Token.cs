using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Airslip.Identity.Api.Auth
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record Token
    {
        public Token(IHttpContextAccessor httpContextAccessor)
            : this(httpContextAccessor.HttpContext!)
        {
        }

        public Token(HttpContext httpContext)
        {
            ClaimsPrincipal claimsPrincipal = httpContext.User;
            List<Claim> claims = claimsPrincipal.Claims.ToList();

            UserId = claims.GetValue("userid");

            string correlationId = claims.GetValue("correlation");
            CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString() : correlationId;

            MerchantName = claims.GetValue("merchantname");
            Identity = claims.GetValue("identity");
            SessionId = claims.GetValue("jti");

            ThirdPartyCorrelationId = claims.GetValue("thirdpartycorrelationid");
            IpAddress = claims.GetValue("ip");
            UserAgent = claims.GetValue("ua");
            IsAuthenticated = claimsPrincipal.Identity?.IsAuthenticated;
            BearerToken = httpContext.Request.Headers["Authorization"];
        }

        public bool? IsAuthenticated { get; init; }

        public string UserId { get; init; }

        // For whitelabel solution
        public string? MerchantName { get; init; }
        public string Identity { get; init; }
        public string SessionId { get; init; }
        public string CorrelationId { get; init; }
        public string ThirdPartyCorrelationId { get; init; }
        public string IpAddress { get; init; }
        public string UserAgent { get; init; }
        public string BearerToken { get; init; }
    }
}
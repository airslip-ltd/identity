using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Airslip.Identity.Api.Auth
{
    public static class ClaimsExtensions
    {
        public static string GetValue(this IEnumerable<Claim> claims, string type)
        {
            Claim? claim = claims.FirstOrDefault(c => c.Type == type);
            return claim != null ? claim.Value : string.Empty;
        }
    }
}
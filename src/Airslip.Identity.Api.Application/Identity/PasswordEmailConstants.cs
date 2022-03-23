namespace Airslip.Identity.Api.Application.Identity
{
    public static class PasswordEmailConstants
    {
        public static string GetPasswordResetUrl(string baseUri, string relativeEndpoint, string token, string email)
        {
            return $"{baseUri}/{relativeEndpoint}?{nameof(token)}={token}&{nameof(email)}={email}";
        }
    }
}
namespace Airslip.Identity.Api.Application.Identity
{
    public static class ForgotPasswordEmailConstants
    {
        public const string Subject = "Your reset password link";

        public static string GetContent(string resetPasswordUrl)
        {
            return $@"Hello, 
            We see that you’d like to change your password. You can do so by following the link below, which is valid for 24 hours. 
            {resetPasswordUrl} 
            If you didn’t make this request, then please contact us by emailing support@airslip.com. 
            Kind regards,
            The Airslip team";
        }

        public static string GetPasswordResetUrl(string baseUri, string relativeEndpoint, string token, string email)
        {
            return $"{baseUri}/{relativeEndpoint}?{nameof(token)}={token}&{nameof(email)}={email}";
        }
    }
}
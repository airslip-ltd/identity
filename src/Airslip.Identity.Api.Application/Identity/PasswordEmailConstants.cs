namespace Airslip.Identity.Api.Application.Identity
{
    public static class PasswordEmailConstants
    {
        public const string ForgotSubject = "Your reset password link";
        public const string NewUserSubject = "Welcome to Airslip";

        public static string GetForgotPlainTextContent()
        {
            return @"
            Hello, 

            We see that you’d like to change your password. You can do so by following the link below, which is valid for 24 hours. 
            {password_url}
             
            If you didn't make this request, then please contact us by emailing support@airslip.com. 
            Kind regards,
            The Airslip team";
        }
        
        public static string GetNewUserPlainTextContent()
        {
            return @"
            Hello, 

            Some content around a new user... 
            {password_url}
             
            If you didn't make this request, then please contact us by emailing support@airslip.com. 
            Kind regards,
            The Airslip team";
        }

        public static string GetPasswordResetUrl(string baseUri, string relativeEndpoint, string token, string email)
        {
            return $"{baseUri}/{relativeEndpoint}?{nameof(token)}={token}&{nameof(email)}={email}";
        }
    }
}
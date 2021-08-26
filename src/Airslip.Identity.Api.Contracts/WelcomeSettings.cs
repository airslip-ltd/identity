namespace Airslip.Identity.Api.Contracts
{
    public class WelcomeSettings
    {
        public WelcomeUser NewUser { get; set; } = new();
        public WelcomeUser ExistingUser { get; set; } = new();
    }

    public class WelcomeUser
    {
        public string Message { get; set; } = string.Empty;
    }
}
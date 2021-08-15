namespace Airslip.Identity.Api.Contracts
{
    public class PublicApiSettings
    {
        public IPublicApiSetting Base { get; set; } = new PublicApiSetting();
        public IPublicApiSetting BankTransactions { get; set; } = new PublicApiSetting();
    }

    public interface IPublicApiSetting
    {
        public string BaseUri { get; set; }
        public string UriSuffix { get; set; }
        public string Version { get; set; }
    }

    public class PublicApiSetting : IPublicApiSetting
    {
        public string BaseUri { get; set; } = string.Empty;
        public string UriSuffix { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }
}
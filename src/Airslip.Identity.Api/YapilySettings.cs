using System.Collections.Generic;

namespace Airslip.Identity.Api
{
    public class YapilySettings
    {
        public string BaseUri { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public string AuthorisationCallbackUrl { get; set; } = string.Empty;
        public ICollection<string> EnvironmentTypes { get; set; } = new List<string>();
    }
}
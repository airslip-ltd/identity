using Airslip.Common.Types.Interfaces;
using Newtonsoft.Json;
using System.Web;

namespace Airslip.Identity.Api.Contracts.Responses
{
    public class CreateUnregisteredUserResponse : ISuccess
    {
        public string ConfirmationLink { get; private set; } = string.Empty;
        [JsonIgnore] 
        public string Email { get; set; } = string.Empty;
        [JsonIgnore]
        public string ConfirmationToken { get; set; }= string.Empty;

        public void GenerateLink(string baseUri, string relativeUri)
        {
            ConfirmationLink = $"{baseUri}/{relativeUri}?email={Email}&token={HttpUtility.UrlEncode(ConfirmationToken)}";
        }
    }
}

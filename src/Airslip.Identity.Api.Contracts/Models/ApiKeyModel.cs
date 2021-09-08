using Airslip.Common.Auth.Enums;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;
using Newtonsoft.Json;

namespace Airslip.Identity.Api.Contracts.Models
{
    public class ApiKeyModel : IModel
    {
        public string? Id { get; set; }
        
        public EntityStatusEnum EntityStatus { get; set; }
        
        [JsonIgnore]
        public string? KeyValue { get; set; }
        
        [JsonProperty("keyValue")]
        private string KeyValueSetter
        {
            set => KeyValue = value;
        }
        
        [JsonIgnore]
        public ApiKeyUsageType ApiKeyUsageType { get; set; }
        
        [JsonIgnore]
        public string? OwningEntityId { get; set; }

        public string? Name { get; set; }
    }
}
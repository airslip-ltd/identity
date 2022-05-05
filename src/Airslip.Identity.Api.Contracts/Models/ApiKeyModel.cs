using Airslip.Common.Repository.Types.Enums;
using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Common.Types.Enums;
using Newtonsoft.Json;

namespace Airslip.Identity.Api.Contracts.Models
{
    public class ApiKeyModel : IModelWithOwnership
    {
        public string? Id { get; set; }
        
        public EntityStatus EntityStatus { get; set; }
        
        [JsonIgnore]
        public string? KeyValue { get; set; }
        
        [JsonIgnore]
        public AirslipUserType AirslipUserType { get; set; }

        [JsonIgnore]
        public string? UserId { get; set; }

        [JsonIgnore]
        public string? EntityId { get; set; }

        public string? Name { get; set; }
        
        public string? TokenValue { get; set; }
        public string? ApiKeyUserId { get; set; }
    }
}
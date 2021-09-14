using Airslip.Common.Auth.Enums;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;
using Newtonsoft.Json;

namespace Airslip.Identity.Api.Contracts.Models
{
    public class QrCodeModel : IModel
    {
        public string? Id { get; set; }
        public EntityStatus EntityStatus { get; set; }
        [JsonIgnore]
        public string? KeyValue { get; set; }
        [JsonIgnore]
        public AirslipUserType AirslipUserType { get; set; }
        [JsonIgnore]
        public string? EntityId { get; set; }
        public string? Name { get; set; }
        public string? StoreId { get; set; }
        public string? CheckoutId { get; set; }
        public string? TokenValue { get; set; }
    }
}
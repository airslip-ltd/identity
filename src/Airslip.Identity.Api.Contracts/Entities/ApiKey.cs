using Airslip.Common.Repository.Types.Entities;
using Airslip.Common.Repository.Types.Enums;
using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Common.Types.Enums;

namespace Airslip.Identity.Api.Contracts.Entities
{
    public record ApiKey : IEntityWithOwnership
    {
        public string Id { get; set; } = "";
        public string? KeyValue { get; set; }
        public BasicAuditInformation? AuditInformation { get; set; }
        public EntityStatus EntityStatus { get; set; }
        public AirslipUserType AirslipUserType { get; set; }
        public string? UserId { get; set; }
        public string? EntityId { get; set; }
        public string? Name { get; set; }
        public string? ApiKeyUserId { get; set; }
    }
}
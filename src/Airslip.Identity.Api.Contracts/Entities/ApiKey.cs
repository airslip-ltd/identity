using Airslip.Common.Auth.Enums;
using Airslip.Common.Repository.Entities;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;

namespace Airslip.Identity.Api.Contracts.Entities
{
    public record ApiKey : IEntity
    {
        public string Id { get; set; } = "";
        public string? KeyValue { get; set; }
        public BasicAuditInformation? AuditInformation { get; set; }
        public EntityStatus EntityStatus { get; set; }
        public AirslipUserType AirslipUserType { get; init; }
        public string? EntityId { get; init; }
        public string? Name { get; set; }
    }
}
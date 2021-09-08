using Airslip.Common.Auth.Enums;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;
using System;

namespace Airslip.Identity.Api.Contracts.Entities
{
    public record ApiKey : IEntity, IEntityWithId
    {
        public string Id { get; set; } = "";
        public string? KeyValue { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public string? UpdatedByUserId { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string? DeletedByUserId { get; set; }
        public DateTime? DateDeleted { get; set; }
        public EntityStatusEnum EntityStatus { get; set; }
        public ApiKeyUsageType ApiKeyUsageType { get; init; }
        public string? OwningEntityId { get; init; }
        public string? Name { get; set; }
    }
}
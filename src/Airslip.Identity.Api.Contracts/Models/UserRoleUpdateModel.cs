using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;
using JetBrains.Annotations;

namespace Airslip.Identity.Api.Contracts.Models
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record UserRoleUpdateModel : IModel
    {
        public string? Id { get; set; }
        public EntityStatus EntityStatus { get; set; }
        public string? UserRole { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
    }
}
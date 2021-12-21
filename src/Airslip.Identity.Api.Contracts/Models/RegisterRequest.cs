using Airslip.Common.Auth.Data;
using Airslip.Common.Types.Enums;
using JetBrains.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Airslip.Identity.Api.Contracts.Models
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record UserAddModel
    {
        [Required] 
        [EmailAddress] 
        public string? Email { get; init; }
        
        [Required]
        public string? FirstName { get; init; }

        [Required]
        public string? LastName { get; init; }

        public string UserRole { get; init; } = UserRoles.User;
        public string? EntityId { get; init; }
        public AirslipUserType AirslipUserType { get; init; } = AirslipUserType.Standard;
    }
}
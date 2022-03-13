using Airslip.Common.Auth.Data;
using Airslip.Common.Types.Enums;
using JetBrains.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Airslip.Identity.Api.Contracts.Requests
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record RegisterRequest
    {
        [Required] 
        [EmailAddress] 
        public string? Email { get; init; }

        [DataType(DataType.Password)]
        public string? Password { get; init; }
        
        [Required]
        public string? DeviceId { get; init; }
        
        [Required]
        public string? FirstName { get; init; }

        [Required]
        public string? LastName { get; init; }

        public string UserRole { get; init; } = UserRoles.User;
        public string? EntityId { get; init; }
        public AirslipUserType AirslipUserType { get; init; } = AirslipUserType.Standard;
    }
}
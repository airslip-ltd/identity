using Airslip.Common.Types.Enums;
using JetBrains.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Airslip.Identity.Api.Contracts.Requests
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record LoginRequest
    {
        [Required] [EmailAddress] public string? Email { get; init; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; init; }
        
        [Required]
        public string? DeviceId { get; init; }
        
        public string? EntityId { get; init; }

        public AirslipUserType AirslipUserType { get; init; } = AirslipUserType.Standard;
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public bool CreateUserIfNotExists { get; init; } = false;
    }
}
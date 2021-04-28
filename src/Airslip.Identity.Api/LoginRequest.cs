using JetBrains.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Airslip.Identity.Api
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record LoginRequest
    {
        [Required] [EmailAddress] public string? Email { get; init; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; init; }
    }
}
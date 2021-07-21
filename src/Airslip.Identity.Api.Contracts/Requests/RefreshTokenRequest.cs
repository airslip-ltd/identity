using JetBrains.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Airslip.Identity.Api.Contracts.Requests
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record RefreshTokenRequest
    {
        [Required] public string? RefreshToken { get; init; }
        [Required] public string? DeviceId { get; init; }
    }
}
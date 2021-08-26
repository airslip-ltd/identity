using JetBrains.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Airslip.Identity.Api.Contracts.Requests
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record CheckUserRequest
    {
        [Required] [EmailAddress] public string? Email { get; init; }
    }
}
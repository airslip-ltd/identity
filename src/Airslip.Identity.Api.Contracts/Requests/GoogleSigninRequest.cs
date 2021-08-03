using JetBrains.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Airslip.Identity.Api.Contracts.Requests
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record GoogleSigninRequest
    {
        [JsonPropertyName("email")] public string? Email { get; init; }
        [JsonPropertyName("name")] public string? Name { get; init; }
        [JsonPropertyName("picture")] public string? PictureUrl { get; init; }
        [JsonPropertyName("locale")] public string? Locale { get; init; }
        [JsonPropertyName("family_name")] public string? FamilyName { get; init; }
        [JsonPropertyName("given_name")] public string? GivenName { get; init; }
        [Required] public string? RefreshToken { get; init; }
        [Required] public string? DeviceId { get; init; }
    }
}
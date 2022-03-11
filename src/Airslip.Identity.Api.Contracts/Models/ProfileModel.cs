using Airslip.Common.Repository.Types.Enums;
using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using JetBrains.Annotations;

namespace Airslip.Identity.Api.Contracts.Models;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public record ProfileModel : IModel
{
    public string? Id { get; set; }
    public EntityStatus EntityStatus { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? DisplayName { get; set; }
    public DataConsent DataConsent { get; set; } = new ();
}
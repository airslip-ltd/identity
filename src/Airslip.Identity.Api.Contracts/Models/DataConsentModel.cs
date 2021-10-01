namespace Airslip.Identity.Api.Contracts.Models
{
    public record DataConsentModel(
        bool Essential,
        bool Performance,
        bool Personalisation);
}
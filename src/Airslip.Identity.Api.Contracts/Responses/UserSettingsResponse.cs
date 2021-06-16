namespace Airslip.Identity.Api.Contracts.Responses
{
    public record UserSettingsResponse(
        bool? HasFaceId,
        bool? isNewUser);
}
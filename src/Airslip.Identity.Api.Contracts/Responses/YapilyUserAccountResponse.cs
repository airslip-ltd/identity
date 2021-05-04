using Airslip.Common.Contracts;

namespace Airslip.Identity.Api.Contracts.Responses
{
    public record YapilyUserAccountResponse(
        string Id,
        string UserId,
        string Institution,
        string InstitutionConsentToken,
        string Nickname) : ISuccess;
}
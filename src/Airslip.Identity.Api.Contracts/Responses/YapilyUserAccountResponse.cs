using Airslip.Common.Contracts;

namespace Airslip.BankTransactions.Api.Contracts.Responses
{
    public record YapilyUserAccountResponse(
        string Id,
        string UserId,
        string Institution,
        string InstitutionConsentToken,
        string Nickname) : ISuccess;
}
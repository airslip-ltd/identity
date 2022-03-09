using Airslip.Common.Types.Interfaces;

namespace Airslip.Identity.Api.Contracts.Models
{
    public record ApiKeyValidationResultModel(bool IsValid, string Message) : ISuccess;
}
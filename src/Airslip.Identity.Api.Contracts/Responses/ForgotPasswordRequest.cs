using Airslip.Common.Types.Interfaces;

namespace Airslip.Identity.Api.Contracts.Responses
{
    public record ForgotPasswordResponse(string ResetPasswordUrl) : ISuccess;
}
using Airslip.Common.Contracts;

namespace Airslip.Identity.Api.Contracts.Responses
{
    public record ForgotPasswordResponse(string ResetPasswordUrl) : ISuccess;
}
using Airslip.Common.Contracts;

namespace Airslip.Identity.Api.Contracts
{
    public record UserResponse(string Message, bool isNewUser) : ISuccess;
}
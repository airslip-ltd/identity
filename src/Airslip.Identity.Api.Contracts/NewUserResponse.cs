using Airslip.Common.Types.Interfaces;

namespace Airslip.Identity.Api.Contracts
{
    public record UserResponse(string Message, bool isNewUser) : ISuccess;
}
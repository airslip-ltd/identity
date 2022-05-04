using Airslip.Common.Types.Interfaces;

namespace Airslip.Identity.Api.Contracts
{
    public record UserResponse(string Email, bool isNewUser) : ISuccess;
}
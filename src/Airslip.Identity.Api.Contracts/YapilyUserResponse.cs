using Airslip.Common.Types.Interfaces;

namespace Airslip.Identity.Api.Contracts
{
    public record YapilyUserResponse(string UserId) : ISuccess;
}
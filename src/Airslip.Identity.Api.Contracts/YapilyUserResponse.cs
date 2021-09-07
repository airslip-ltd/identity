using Airslip.Common.Contracts;

namespace Airslip.Identity.Api.Contracts
{
    public record YapilyUserResponse(string UserId) : ISuccess;
}
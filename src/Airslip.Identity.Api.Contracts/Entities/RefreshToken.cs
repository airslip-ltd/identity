using Airslip.Common.Repository.Interfaces;

namespace Airslip.Identity.Api.Contracts.Entities
{
    public record RefreshToken(string DeviceId, string Token) : IEntityNoId;
}
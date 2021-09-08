using Airslip.Common.Repository.Interfaces;

namespace Airslip.Identity.MongoDb.Contracts.Entities
{
    public record RefreshToken(string DeviceId, string Token) : IEntityNoId;
}
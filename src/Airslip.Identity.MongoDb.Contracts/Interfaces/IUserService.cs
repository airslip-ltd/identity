using Airslip.Identity.MongoDb.Contracts.Entities;
using System.Threading.Tasks;

namespace Airslip.Identity.MongoDb.Contracts.Interfaces
{
    public interface IUserService
    {
        Task<User?> Get(string id);
        Task<string?> GetProviderId(string id, string provider);
        Task<User> Create(User user);
        Task Update(User userIn);
        Task UpdateRefreshToken(string id, string deviceId, string token);
        Task ToggleBiometric(string id, bool biometricOn);
    }
}
using System.Threading.Tasks;

namespace Airslip.Identity.MongoDb.Contracts
{
    public interface IUserService
    {
        Task<User> Get(string id);
        Task<User?> GetByEmail(string email);
        Task Create(User user);
        Task UpdateRefreshToken(string userId, string deviceId, string token);
    }
}
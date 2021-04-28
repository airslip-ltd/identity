using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airslip.Identity.MongoDb.Contracts
{
    public interface IUserService
    {
        Task<User> Get(string id);
        Task<User?> GetByEmail(string email);
        Task<List<User>> GetUsersWithNoConsentToken();
        Task Create(User user);
        Task Update(User userIn);
        Task UpdatePreviousViewedAccountId(string userId, string accountId);
    }
}
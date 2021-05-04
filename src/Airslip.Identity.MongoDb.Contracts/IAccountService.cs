using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airslip.Identity.MongoDb.Contracts
{
    public interface IAccountService
    {
        Task<List<Account>> GetAccountsForUser(string userId);
    }
}
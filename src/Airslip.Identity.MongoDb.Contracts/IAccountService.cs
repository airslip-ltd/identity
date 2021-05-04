using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airslip.BankTransactions.MongoDb.Contracts
{
    public interface IAccountService
    {
        Task<Account?> Get(string userId, string accountId);
        Task<List<Account>> GetAccountsForUser(string userId);
        Task Update(Account accountIn);
        Task Upsert(IEnumerable<Account> accountsIn);
    }
}
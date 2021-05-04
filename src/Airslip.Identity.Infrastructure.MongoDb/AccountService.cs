using Airslip.Identity.MongoDb.Contracts;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airslip.Identity.Infrastructure.MongoDb
{
    public class AccountService : IAccountService
    {
        private readonly AirslipMongoDbContext _context;

        public AccountService(AirslipMongoDbContext context)
        {
            _context = context;
        }

        public Task<List<Account>> GetAccountsForUser(string userId)
        {
            return _context.Accounts.Find(account => account.UserId == userId).ToListAsync();
        }
    }
}
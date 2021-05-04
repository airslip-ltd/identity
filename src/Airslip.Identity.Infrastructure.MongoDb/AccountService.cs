using Airslip.BankTransactions.MongoDb.Contracts;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airslip.BankTransactions.Infrastructure.MongoDb.Services
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

        public async Task<Account?> Get(string userId, string accountId)
        {
            return await _context.Accounts.Find(account => account.Id == CompositeId.Build(userId, accountId))
                .FirstOrDefaultAsync();
        }

        public async Task Upsert(IEnumerable<Account> accountsIn)
        {
            foreach (Account? accountIn in accountsIn)
                await Upsert(accountIn);
        }

        public Task Update(Account accountIn)
        {
            return _context.Accounts.ReplaceOneAsync(
                account => account.Id == CompositeId.Build(accountIn.UserId, accountIn.AccountId), accountIn);
        }

        private Task Upsert(Account accountIn)
        {
            return _context.Accounts.ReplaceOneAsync(
                account => account.Id == CompositeId.Build(accountIn.UserId, accountIn.AccountId), accountIn,
                new ReplaceOptions { IsUpsert = true });
        }
    }
}
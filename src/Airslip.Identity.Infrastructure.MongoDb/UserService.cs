using Airslip.Identity.MongoDb.Contracts;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airslip.Identity.Infrastructure.MongoDb
{
    public class UserService : IUserService
    {
        private readonly AirslipMongoDbContext _context;

        public UserService(AirslipMongoDbContext context)
        {
            _context = context;
        }

        public Task<User> Get(string id)
        {
            return _context.Users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users.Find(user => user.EmailAddress == email).FirstOrDefaultAsync();
        }

        public async Task<bool> DoesUserExist(string email)
        {
            return await GetByEmail(email) is not null;
        }

        public Task<List<User>> GetUsersWithNoConsentToken()
        {
            FilterDefinitionBuilder<User> filterBuilder = Builders<User>.Filter;
            FilterDefinition<User> filter = filterBuilder.ElemMatch(user => user.Institutions,
                institution => institution.ConsentToken == null);
            return _context.Users.Find(filter).ToListAsync();
        }

        public Task Create(User user)
        {
            return _context.Users.InsertOneAsync(user);
        }

        public Task Update(User userIn)
        {
            return _context.Users.ReplaceOneAsync(user => user.Id == userIn.Id, userIn);
        }

        public Task UpdatePreviousViewedAccountId(string userId, string accountId)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(user => user.Id, userId);
            UpdateDefinition<User> update = Builders<User>.Update.Set(user => user.PreviousViewedAccountId, accountId);
            return _context.Users.UpdateOneAsync(filter, update);
        }

        public async Task UpdateRefreshToken(string userId, string deviceId, string token)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            User userIn = await _context.Users.Find(filter).FirstOrDefaultAsync();

            RefreshToken? refreshToken = userIn.RefreshTokens?.FirstOrDefault(rt => rt.DeviceId == deviceId);

            if (refreshToken != null)
                userIn.RefreshTokens?.Remove(refreshToken);

            userIn.AddRefreshToken(deviceId, token);

            await _context.Users.ReplaceOneAsync(
                user => user.Id == userId, userIn,
                new ReplaceOptions { IsUpsert = true });
        }
    }
}
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using MongoDB.Driver;
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

        public async Task<User?> Get(string id)
        {
            return await _context.Users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task<string?> GetProviderId(string id, string provider)
        {
            User? user = await _context.Users.Find(user => user.Id == id).FirstOrDefaultAsync();
            return user.OpenBankingProviders.Where(o => o.Name == provider)
                .Select(u => u.Id)
                .FirstOrDefault();
        }

        public async Task<User> Create(User user)
        {
            await _context.Users.InsertOneAsync(user);
            return user;
        }

        public Task Update(User userIn)
        {
            return _context.Users.ReplaceOneAsync(user => user.Id == userIn.Id, userIn);
        }

        public async Task UpdateOrReplaceRefreshToken(string id, string deviceId, string token)
        {
            FilterDefinitionBuilder<User>? filterDefinitionBuilder = Builders<User>.Filter;
            FilterDefinition<User> userFilter = filterDefinitionBuilder.Eq(user => user.Id, id);
            FilterDefinition<User> filter = userFilter & filterDefinitionBuilder.Eq("refreshTokens.deviceId", deviceId);

            UpdateDefinition<User> update = Builders<User>.Update.Set("refreshTokens.$.token", token);

            UpdateResult updateResult = await _context.Users.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount > 0)
                return;

            // This is only invoked when a new device is registered
            User userIn = await _context.Users.Find(userFilter).FirstOrDefaultAsync();

            userIn.AddRefreshToken(deviceId, token);

            await _context.Users.ReplaceOneAsync(
                user => user.Id == id, userIn,
                new ReplaceOptions { IsUpsert = true });
        }

        public Task ToggleBiometric(string id, bool biometricOn)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(user => user.Id, id);
            UpdateDefinition<User> update = Builders<User>.Update.Set(user => user.BiometricOn, biometricOn);
            return _context.Users.UpdateOneAsync(filter, update);
        }
    }
}
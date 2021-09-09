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
            User? user =await  _context.Users.Find(user => user.Id == id).FirstOrDefaultAsync();
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

        public async Task UpdateRefreshToken(string id, string deviceId, string token)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Id, id);
            User userIn = await _context.Users.Find(filter).FirstOrDefaultAsync();

            RefreshToken? refreshToken = userIn.RefreshTokens.FirstOrDefault(rt => rt.DeviceId == deviceId);

            if (refreshToken != null)
                userIn.RefreshTokens.Remove(refreshToken);

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
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Identity.MongoDb.Contracts.Entities;
using Airslip.Identity.MongoDb.Contracts.Interfaces;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Airslip.Identity.Infrastructure.MongoDb
{
    public class UserProfileService : IUserProfileService
    {
        private readonly AirslipMongoDbContext _context;

        public UserProfileService(AirslipMongoDbContext context)
        {
            _context = context;
        }

        public Task<UserProfile> Get(string userId)
        {
            return _context.UserProfiles.Find(user => user.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<UserProfile?> GetByEmail(string email)
        {
            return await _context.UserProfiles.Find(user => user.Email == email).FirstOrDefaultAsync();
        }

        public Task Create(UserProfile userProfileIn)
        {
            return _context.UserProfiles.InsertOneAsync(userProfileIn);
        }

        public Task Update(UserProfile userProfileIn)
        {
            return _context.UserProfiles.ReplaceOneAsync(userProfile => userProfile.UserId == userProfileIn.UserId, userProfileIn);
        }
    }
}
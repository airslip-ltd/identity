using Airslip.Identity.MongoDb.Contracts;
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
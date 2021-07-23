using System.Threading.Tasks;

namespace Airslip.Identity.MongoDb.Contracts
{
    public interface IUserProfileService
    {
        Task<UserProfile> Get(string userId);
        Task<UserProfile?> GetByEmail(string email);
        Task Create(UserProfile userProfileIn);
        Task Update(UserProfile userProfileIn);
    }
}
using Airslip.Identity.Api.Contracts.Entities;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfile> Get(string userId);
        Task<UserProfile?> GetByEmail(string email);
        Task Create(UserProfile userProfileIn);
        Task Update(UserProfile userProfileIn);
    }
}
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Identity.MongoDb.Contracts.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Airslip.Identity.Infrastructure.MongoDb.Identity
{
    public class UserManagerService : IUserManagerService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagerService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        public async Task<bool> TryToLogin(string email, string password)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            return user is not null && await _userManager.CheckPasswordAsync(user, password);
        }
        
        public async Task<IdentityResult> Create(string email, string password)
        {
            ApplicationUser applicationUser = new() {UserName = email, Email = email};
                
            IdentityResult result = await _userManager.CreateAsync(applicationUser, password);

            return result;
        }
    }
}
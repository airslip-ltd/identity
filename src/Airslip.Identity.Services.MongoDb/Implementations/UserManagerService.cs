using Airslip.Identity.Services.MongoDb.Identity;
using Airslip.Identity.Services.MongoDb.Identity.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Airslip.Identity.Services.MongoDb.Implementations
{
    public class UserManagerService : IUserManagerService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagerService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> TryToLogin(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityResult> Create(string email, string password)
        {
            ApplicationUser applicationUser = new() { UserName = email, Email = email };

            IdentityResult result = await _userManager.CreateAsync(applicationUser, password);

            return result;
        }

        public async Task<ApplicationUser?> FindByEmail(string email)
        {
           return await _userManager.FindByEmailAsync(email);
        }

        public Task<string> GeneratePasswordResetToken(ApplicationUser user) => _userManager.GeneratePasswordResetTokenAsync(user);
        
        public Task<IdentityResult> ResetPassword(ApplicationUser user, string token, string password)
        {
           return _userManager.ResetPasswordAsync(user, token, password);
        }
    }
}
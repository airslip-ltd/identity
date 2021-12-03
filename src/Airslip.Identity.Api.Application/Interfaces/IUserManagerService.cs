using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IUserManagerService
    {
        Task<bool> TryToLogin(ApplicationUser user, string password);
        Task<IdentityResult> Create(string email, string password);
        Task<ApplicationUser?> FindByEmail(string email);
        Task<string> GeneratePasswordResetToken(ApplicationUser user);
        Task<IdentityResult> ResetPassword(ApplicationUser user, string token, string password);
        Task<IResponse> SetRole(string userId, string userRole);
        Task<string[]> GetRoles(string userEmail);
    }
}
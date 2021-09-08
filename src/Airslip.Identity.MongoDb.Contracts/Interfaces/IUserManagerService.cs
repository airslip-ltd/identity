﻿using Airslip.Identity.MongoDb.Contracts.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Airslip.Identity.MongoDb.Contracts.Interfaces
{
    public interface IUserManagerService
    {
        Task<bool> TryToLogin(ApplicationUser user, string password);
        Task<IdentityResult> Create(string email, string password);
        Task<ApplicationUser?> FindByEmail(string email);
        Task<string> GeneratePasswordResetToken(ApplicationUser user);
        Task<IdentityResult> ResetPassword(ApplicationUser user, string token, string password);
    }
}
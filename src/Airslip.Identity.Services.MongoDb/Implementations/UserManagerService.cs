using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Models;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Data;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Airslip.Identity.Services.MongoDb.Implementations
{
    public class UserManagerService : IUserManagerService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRepository<User, UserRoleUpdateModel> _repository;

        public UserManagerService(UserManager<ApplicationUser> userManager, 
            RoleManager<ApplicationRole> roleManager,
            IRepository<User, UserRoleUpdateModel> repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _repository = repository;
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

        public async Task<IResponse> SetRole(string userId, string userRole)
        {
            RepositoryActionResultModel<UserRoleUpdateModel> result = await _repository
                .Get(userId);

            if (result is not SuccessfulActionResultModel<UserRoleUpdateModel>) 
                return result;

            UserRoleUpdateModel updateModel = result.CurrentVersion!;
            updateModel.UserRole = userRole;

            result = await _repository.Update(updateModel.Id!, updateModel);

            if (result is not SuccessfulActionResultModel<UserRoleUpdateModel>) 
                return result;
            
            ApplicationUser? user = await _userManager.FindByEmailAsync(updateModel.Email);

            if (user == null)
                throw new ArgumentException("User not found", nameof(userId));
            
            user.Roles.Clear();

            foreach (string applicationRoles in RoleHelper.GetRoleProfile(userRole))
            {
                ApplicationRole applicationRole = await _roleManager
                    .FindByNameAsync(applicationRoles);
                user.AddRole(applicationRole.Id);
            }

            await _userManager.UpdateAsync(user);

            return result;
        }

        public async Task<string[]> GetRoles(string userEmail)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(userEmail);

            var roles = _roleManager
                .Roles
                .Where(o => user.Roles.Contains(o.Id))
                .Select(o => o.Name);

            return roles.ToArray();
        }
    }
}
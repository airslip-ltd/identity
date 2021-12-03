using Airslip.Common.Auth.Data;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Contracts.Data
{
    public static class RoleHelper
    {
        public static List<string> GetRoleProfile(string role)
        {
            List<string> roles = new();

            switch (role)
            {
                case UserRoles.Administrator:
                    roles.Add(ApplicationRoles.UserManager);
                    break;
                case UserRoles.Manager:
                    roles.Add(ApplicationRoles.UserManager);
                    break;
                case UserRoles.User:
                    break;
            }

            return roles;
        }
    }
}
using AspNetCore.Identity.MongoDbCore.Models;
using JetBrains.Annotations;
using System;

namespace Airslip.Identity.Api.Contracts.Entities
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ApplicationRole : MongoIdentityRole<Guid>
    {
        public ApplicationRole()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
        }
    }
}
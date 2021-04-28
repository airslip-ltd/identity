using AspNetCore.Identity.MongoDbCore.Models;
using JetBrains.Annotations;
using System;

namespace Airslip.Identity.Infrastructure.MongoDb.Identity
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
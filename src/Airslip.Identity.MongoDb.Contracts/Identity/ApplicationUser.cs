using AspNetCore.Identity.MongoDbCore.Models;
using System;

namespace Airslip.Identity.MongoDb.Contracts.Identity
{
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
    }
}
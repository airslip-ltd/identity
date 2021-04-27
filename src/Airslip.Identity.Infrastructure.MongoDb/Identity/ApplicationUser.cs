using AspNetCore.Identity.MongoDbCore.Models;
using System;

namespace Airslip.Identity.Infrastructure.MongoDb.Identity
{
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
    }
}
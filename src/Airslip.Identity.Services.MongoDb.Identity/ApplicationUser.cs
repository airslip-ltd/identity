using AspNetCore.Identity.MongoDbCore.Models;
using System;

namespace Airslip.Identity.Services.MongoDb.Identity
{
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
    }
}
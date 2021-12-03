using AspNetCore.Identity.MongoDbCore.Models;
using System;

namespace Airslip.Identity.Api.Contracts.Entities
{
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
    }
}
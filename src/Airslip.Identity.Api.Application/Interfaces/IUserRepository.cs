using Airslip.Common.Repository.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IUserRepository : IRepository<User, UserModel>
    {
    }
}
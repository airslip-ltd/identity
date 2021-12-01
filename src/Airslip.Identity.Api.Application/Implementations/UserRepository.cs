using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Implementations;
using Airslip.Common.Repository.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class UserRepository : Repository<User, UserModel>, IUserRepository
    {
        public UserRepository(
            IContext context, 
            IModelValidator<UserModel> validator, 
            IModelMapper<UserModel> mapper,
            IEnumerable<IModelDeliveryService<UserModel>> deliveryServices,
            IEnumerable<IEntitySearchFormatter<UserModel>> searchFormatters, 
            IRepositoryUserService repositoryUserService) : 
            base(context, validator, mapper, deliveryServices, searchFormatters, repositoryUserService)
        {
        }
    }
}
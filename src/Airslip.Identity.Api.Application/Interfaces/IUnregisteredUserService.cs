using Airslip.Common.Repository.Models;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IUnregisteredUserService
    {
        Task<RepositoryActionResultModel<UserModel>> Create(CreateUnregisteredUserModel createModel);
    }
}
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IUnregisteredUserService
    {
        Task<IResponse> Create(CreateUnregisteredUserModel createModel);
        Task<IResponse> ConfirmEmail(string email, string token);
    }
}
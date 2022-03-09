using Airslip.Common.Repository.Types.Models;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces;

public interface IUserService
{
    Task<IResponse> Update(string id, UserModel model, string? userId = null);
    Task<IResponse> Delete(string id, string? userId = null);
    Task<IResponse> Get(string id);
    Task<IResponse> Search(EntitySearchQueryModel query);
}
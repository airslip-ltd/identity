using Airslip.Common.Repository.Types.Models;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IApiKeyService
    {
        Task<IResponse> Search(EntitySearchQueryModel query);
        Task<IResponse> CreateNewApiKey(CreateApiKeyModel createApiKeyModel);
        Task<IResponse> Delete(string id);
        Task<IResponse> ValidateApiKey(ApiKeyValidationModel model);
        Task<IResponse> Get(string id);
    }
}
using Airslip.Common.Repository.Models;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IApiKeyService
    {
        Task<RepositoryActionResultModel<ApiKeyModel>> CreateNewApiKey(CreateApiKeyModel createApiKeyModel);
        Task<RepositoryActionResultModel<ApiKeyModel>> ExpireApiKey(string id);
    }
}
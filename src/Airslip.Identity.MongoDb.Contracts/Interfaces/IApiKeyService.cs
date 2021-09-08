using Airslip.Identity.MongoDb.Contracts.Entities;
using System.Threading.Tasks;

namespace Airslip.Identity.MongoDb.Contracts.Interfaces
{
    public interface IApiKeyService
    {
        Task<ApiKey?> Get(string id);
        Task<ApiKey> Create(ApiKey apiKey);
        Task Update(ApiKey apiKey);
        Task Delete(ApiKey apiKey);
    }
}
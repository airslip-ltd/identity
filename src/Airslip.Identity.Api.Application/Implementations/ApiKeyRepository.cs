using Airslip.Common.Repository.Implementations;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using MongoDB.Driver.Linq;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class ApiKeyRepository : Repository<ApiKey, ApiKeyModel>, IApiKeyRepository
    {
        private readonly IContext _context;

        public ApiKeyRepository(IContext context, IModelMapper<ApiKeyModel> mapper, 
            IRepositoryLifecycle<ApiKey, ApiKeyModel> repositoryLifecycle) : 
            base(context, mapper, repositoryLifecycle)
        {
            _context = context;
        }

        public async Task<ApiKey?> GetApiKeyByKeyValue(string keyValue)
        {
            // Get the key
            ApiKey? apiKey = await ((IMongoQueryable<ApiKey>)_context
                .QueryableOf<ApiKey>())
                .FirstOrDefaultAsync(o => o.KeyValue == keyValue);

            return apiKey;
        }


    }
}
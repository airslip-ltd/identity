using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Implementations;
using Airslip.Common.Repository.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class ApiKeyRepository : Repository<ApiKey, ApiKeyModel>, IApiKeyRepository
    {
        private readonly IContext _context;

        public ApiKeyRepository(IContext context, IModelValidator<ApiKeyModel> validator, 
            IModelMapper<ApiKeyModel> mapper, ITokenDecodeService<UserToken> tokenService) 
            : base(context, validator, mapper, tokenService)
        {
            _context = context;
        }

        public async Task<ApiKey?> GetApiKeyByKeyValue(string keyValue)
        {
            // Get the key
            var apiKey = await ((IMongoQueryable<ApiKey>)_context
                .QueryableOf<ApiKey>())
                .FirstOrDefaultAsync(o => o.KeyValue == keyValue);

            return apiKey;
        }
    }
}
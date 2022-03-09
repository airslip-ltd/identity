using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IApiKeyRepository : IRepository<ApiKey, ApiKeyModel>
    {
        Task<ApiKey?> GetApiKeyByKeyValue(string keyValue);
    }
}
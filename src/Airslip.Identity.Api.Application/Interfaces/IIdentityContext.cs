using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IIdentityContext : IContext
    {
        Task<User?> GetByEmail(string email);
        Task UpdateOrReplaceRefreshToken(string id, string deviceId, string token);
    }
}
using Airslip.Common.Types.Interfaces;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IUserSearchService
    {
        Task<IResponse> FindUsers();
    }
}
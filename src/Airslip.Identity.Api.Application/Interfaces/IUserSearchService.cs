using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Identity;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IUserSearchService
    {
        Task<IResponse> FindUsers();
    }
}
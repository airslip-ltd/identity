using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IDataConsentService
    {
        Task<IResponse> Update(DataConsentModel dataConsentModel);
    }
}
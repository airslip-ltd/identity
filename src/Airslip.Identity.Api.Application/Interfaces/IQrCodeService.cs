using Airslip.Common.Repository.Types.Models;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IQrCodeService
    {
        Task<RepositoryActionResultModel<QrCodeModel>> CreateNewQrCode(CreateQrCodeModel createQrCodeModel);
    }
}
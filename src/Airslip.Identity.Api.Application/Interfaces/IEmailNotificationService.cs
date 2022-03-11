using Airslip.Common.Types.Interfaces;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces;

public interface IEmailNotificationService
{
    Task<IResponse> SendPasswordReset(string email, string relativeEndpoint);
    Task<IResponse> SendNewUserEmail(string email, string relativeEndpoint);
}
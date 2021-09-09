using Airslip.Common.Auth.Enums;
using Airslip.Common.Contracts;
using Airslip.Identity.Api.Contracts.Entities;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IUserLoginService
    {
        Task<IResponse> GenerateRefreshToken(string userId, string deviceId, string currentToken);

        Task<IResponse> GenerateUserResponse(User user, bool isNewUser,
            string? yapilyUserId = null, string deviceId = "");
    }
}
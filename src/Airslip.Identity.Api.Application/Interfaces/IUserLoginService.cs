using Airslip.Common.Contracts;
using Airslip.Identity.MongoDb.Contracts;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IUserLoginService
    {
        Task<IResponse> GenerateRefreshToken(string userId, string deviceId, string currentToken);

        Task<IResponse> GenerateUserResponse(User user, bool isNewUser,
            string? yapilyUserId = null, string deviceId = "", string identity = "");
    }
}
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Identity;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces
{
    public interface IUserService
    {
        Task<IResponse> GenerateRefreshToken(string userId, string deviceId, string currentToken);
        Task<IResponse> GenerateRefreshToken(string deviceId, string currentToken);
        Task<IResponse> GenerateUserResponse(User user, bool isNewUser,
            string? yapilyUserId = null, string deviceId = "");
        Task<IResponse> Add(RegisterUserCommand model, CancellationToken cancellationToken, string? userId = null);

        Task<IResponse> Register(RegisterUserCommand model, CancellationToken cancellationToken,
            string? userId = null);
        Task<IResponse> Update(string id, UserModel model, string? userId = null);
        Task<IResponse> Delete(string id, string? userId = null);
        Task<IResponse> Get(string id);
        Task<IResponse> SetRole(string id, string roleName);
    }
}
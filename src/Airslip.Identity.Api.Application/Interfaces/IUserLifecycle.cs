using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Identity;
using Airslip.Identity.Api.Contracts.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Interfaces;

public interface IUserLifecycle
{
    Task<IResponse> GenerateRefreshToken(string deviceId, string currentToken);
    Task<IResponse> GenerateUserResponse(User user, bool isNewUser, string deviceId = "");
    Task<IResponse> Add(RegisterUserCommand model, CancellationToken cancellationToken, string? userId = null);
    Task<IResponse> Register(RegisterUserCommand model, CancellationToken cancellationToken,
        string? userId = null);
    Task<IResponse> SetRole(string id, string roleName);
    Task<string> AddApiUser(string apiKeyName);
}
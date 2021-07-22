using Airslip.Common.Contracts;
using MediatR;

namespace Airslip.Identity.Api.Application.Commands
{
    public class GenerateRefreshTokenCommand : IRequest<IResponse>
    {
        public string UserId { get; }
        public string DeviceId { get; }
        public string Token { get; }

        public GenerateRefreshTokenCommand(string userId, string? deviceId, string? token)
        {
            UserId = userId;
            DeviceId = deviceId ?? string.Empty;
            Token =token ?? string.Empty;
        }
    }
}
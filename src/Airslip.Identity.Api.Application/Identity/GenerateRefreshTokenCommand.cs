using Airslip.Common.Types.Interfaces;
using MediatR;

namespace Airslip.Identity.Api.Application.Identity
{
    public class GenerateRefreshTokenCommand : IRequest<IResponse>
    {
        public string DeviceId { get; }
        public string Token { get; }

        public GenerateRefreshTokenCommand(string? deviceId, string? token)
        {
            DeviceId = deviceId ?? string.Empty;
            Token =token ?? string.Empty;
        }
    }
}
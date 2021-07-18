using Airslip.Common.Contracts;
using MediatR;

namespace Airslip.Identity.Api.Application.Commands
{
    public class GenerateRefreshTokenCommand : IRequest<IResponse>
    {
        public string UserId { get; }
        public string Token { get; }

        public GenerateRefreshTokenCommand(string userId, string? refreshToken)
        {
            UserId = userId;
            Token = refreshToken ?? string.Empty;
        }
    }
}
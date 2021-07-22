using Airslip.Common.Contracts;
using MediatR;

namespace Airslip.Identity.Api.Application.Commands
{
    public class LoginUserCommand : IRequest<IResponse>
    {
        public string Email { get; }
        public string Password { get; } 
        public string DeviceId { get; } 
        
        public LoginUserCommand(string? email, string? password, string? deviceId)
        {
            Email = email ?? string.Empty;
            Password = password ?? string.Empty;
            DeviceId = deviceId ?? string.Empty;
        }
    }
}
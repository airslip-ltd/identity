using Airslip.Common.Contracts;
using MediatR;

namespace Airslip.Identity.Api.Application.Identity
{
    public class LoginUserCommand : IRequest<IResponse>, IAuthenticateRequest
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

    public interface IAuthenticateRequest
    {
        public string Email { get; }
    }
}
using Airslip.Common.Contracts;
using MediatR;

namespace Airslip.Identity.Api.Application.Commands
{
    public class LoginUserCommand : IRequest<IResponse>
    {
        public string Email { get; }
        public string Password { get; } 
        
        public LoginUserCommand(string? email, string? password)
        {
            Email = email ?? string.Empty;
            Password = password ?? string.Empty;
        }
    }
}
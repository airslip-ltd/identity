using Airslip.Common.Contracts;
using MediatR;

namespace Airslip.Identity.Api.Application.Commands
{
    public class GenerateJwtBearerTokenCommand : IRequest<IResponse>
    {
        public string Email { get; }
        public string Password { get; } 
        
        public GenerateJwtBearerTokenCommand(string? email, string? password)
        {
            Email = email ?? string.Empty;
            Password = password ?? string.Empty;
        }
    }
}
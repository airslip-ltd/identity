using Airslip.Common.Types.Interfaces;
using MediatR;

namespace Airslip.Identity.Api.Application.Identity
{
    public class CheckUserCommand : IRequest<IResponse>, IAuthenticateRequest
    {
        public string Email { get; }
        
        public CheckUserCommand(string? email)
        {
            Email = email ?? string.Empty;
        }
    }
}
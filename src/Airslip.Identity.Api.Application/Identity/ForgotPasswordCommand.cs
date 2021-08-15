using Airslip.Common.Contracts;
using MediatR;

namespace Airslip.Identity.Api.Application.Identity
{
    public class ForgotPasswordCommand : IRequest<IResponse>, IAuthenticateRequest
    {
        public string RelativeEndpoint { get; }
        public string Email { get; }
        
        public ForgotPasswordCommand(string relativeEndpoint, string? email)
        {
            RelativeEndpoint = relativeEndpoint;
            Email = email ?? string.Empty;
        }
    }
}
using Airslip.Common.Contracts;
using MediatR;

namespace Airslip.Identity.Api.Application.Identity
{
    public class ResetPasswordCommand : IRequest<IResponse>, IAuthenticateRequest
    {
        public string Password { get; }
        public string ConfirmPassword { get; }
        public string Email { get; }
        public string Token { get; }

        public ResetPasswordCommand(string? password, string? confirmPassword, string? email, string? token)
        {
            Password = password ?? string.Empty;
            ConfirmPassword = confirmPassword ?? string.Empty;
            Email = email ?? string.Empty;
            Token = token ?? string.Empty;
        }
    }
}
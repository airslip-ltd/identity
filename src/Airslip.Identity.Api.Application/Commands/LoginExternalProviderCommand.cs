using Airslip.Common.Contracts;
using MediatR;
using System;

namespace Airslip.Identity.Api.Application.Commands
{
    public class LoginExternalProviderCommand : IRequest<IResponse>
    {
        public string Email { get; }
        public string Provider { get; }
        public string ReferenceId { get; } = Guid.NewGuid().ToString("N");

        public LoginExternalProviderCommand(string? email, string provider)
        {
            Email = email ?? string.Empty;
            Provider = provider;
        }
    }
}
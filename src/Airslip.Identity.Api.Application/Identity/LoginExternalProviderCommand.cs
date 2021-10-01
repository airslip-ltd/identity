using Airslip.Common.Types.Interfaces;
using MediatR;
using System;

namespace Airslip.Identity.Api.Application.Identity
{
    public class LoginExternalProviderCommand : IRequest<IResponse>
    {
        public string Email { get; }
        public string Provider { get; }
        public string DeviceId { get; }
        public string ReferenceId { get; } = Guid.NewGuid().ToString("N");

        public LoginExternalProviderCommand(string? email, string provider, string? deviceId)
        {
            Email = email ?? string.Empty;
            Provider = provider;
            DeviceId = deviceId ?? string.Empty;
        }
    }
}
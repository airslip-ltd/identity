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
        public string? LastName { get; }
        public string? FirstName { get; }

        public LoginExternalProviderCommand(string? email, string provider, string? deviceId, string? firstName, 
            string? lastName)
        {
            Email = email ?? string.Empty;
            Provider = provider;
            LastName = lastName;
            FirstName = firstName;
            DeviceId = deviceId ?? string.Empty;
        }
    }
}
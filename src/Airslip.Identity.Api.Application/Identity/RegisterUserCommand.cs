using Airslip.Common.Types.Interfaces;
using MediatR;
using System;

namespace Airslip.Identity.Api.Application.Identity
{
    public class RegisterUserCommand : IRequest<IResponse>, IAuthenticateRequest
    {
        public RegisterUserCommand(
            string? applicationUserId,
            string? firstName, 
            string? lastName,
            string? password, 
            string? deviceId, 
            string? userRole)
        {
            Email = applicationUserId ?? string.Empty;
            Password = password;
            DeviceId = deviceId ?? string.Empty;
            LastName = lastName;
            UserRole = userRole;
            FirstName = firstName;
        }

        public string Email { get; }
        public string? Password { get; }
        public string DeviceId { get; }
        public string ReferenceId { get; } = Guid.NewGuid().ToString("N");
        public string? LastName { get; }
        public string? UserRole { get; }
        public string? FirstName { get; }
    }
}
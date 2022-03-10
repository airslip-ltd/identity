using Airslip.Common.Types.Enums;
using Airslip.Common.Types.Interfaces;
using MediatR;
using System;

namespace Airslip.Identity.Api.Application.Identity
{
    public class RegisterUserCommand : IRequest<IResponse>, IAuthenticateRequest
    {
        public RegisterUserCommand(
            string? email,
            string? firstName, 
            string? lastName,
            string? password, 
            string? deviceId, 
            string? userRole,
            string? displayName = null)
        {
            Email = email ?? string.Empty;
            Password = password;
            DeviceId = deviceId ?? string.Empty;
            LastName = lastName;
            UserRole = userRole;
            FirstName = firstName;
            DisplayName = displayName;
        }

        public string Email { get; }
        public string? Password { get; }
        public string DeviceId { get; }
        public string ReferenceId { get; } = Guid.NewGuid().ToString("N");
        public string? LastName { get; }
        public string? UserRole { get; }
        public string? FirstName { get; }
        public string? EntityId { get; init; }
        public AirslipUserType AirslipUserType { get; init; } = AirslipUserType.Standard;
        public string? DisplayName { get; }
    }
}
using Airslip.Common.Types.Enums;
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
            string? entityId, 
            AirslipUserType airslipUserType)
        {
            Email = applicationUserId ?? string.Empty;
            Password = password ?? string.Empty;
            DeviceId = deviceId ?? string.Empty;
            EntityId = entityId ?? string.Empty;
            AirslipUserType = airslipUserType;
            LastName = lastName;
            FirstName = firstName;
        }

        public string Email { get; }
        public string Password { get; }
        public string DeviceId { get; }
        public string? EntityId { get; }
        public AirslipUserType AirslipUserType { get; }
        public string ReferenceId { get; } = Guid.NewGuid().ToString("N");
        public string? LastName { get; }
        public string? FirstName { get; }
    }
}
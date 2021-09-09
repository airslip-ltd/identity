using Airslip.Common.Auth.Enums;
using Airslip.Common.Contracts;
using MediatR;
using System;

namespace Airslip.Identity.Api.Application.Identity
{
    public class RegisterUserCommand : IRequest<IResponse>, IAuthenticateRequest
    {
        public RegisterUserCommand(
            string? applicationUserId, 
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
        }

        public string Email { get; }
        public string Password { get; }
        public string DeviceId { get; }
        public string? EntityId { get; }
        public AirslipUserType AirslipUserType { get; }
        public string ReferenceId { get; } = Guid.NewGuid().ToString("N");
    }
}
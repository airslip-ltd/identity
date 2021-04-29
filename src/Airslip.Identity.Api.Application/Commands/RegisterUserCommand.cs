using Airslip.Common.Contracts;
using MediatR;
using System;

namespace Airslip.Identity.Api.Application.Commands
{
    public class RegisterUserCommand : IRequest<IResponse>
    {
        public RegisterUserCommand(string? applicationUserId, string? password)
        {
            Email = applicationUserId ?? string.Empty;
            Password = password ?? string.Empty;
        }

        public string Email { get; }
        public string Password { get; }
        public string ReferenceId { get; } = Guid.NewGuid().ToString("N");
    }
}
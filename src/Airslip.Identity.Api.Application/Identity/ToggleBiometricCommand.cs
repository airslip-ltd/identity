using Airslip.Common.Contracts;
using MediatR;

namespace Airslip.Identity.Api.Application.UserProfiles
{
    public class ToggleBiometricCommand : IRequest<IResponse>
    {
        public string UserId { get; }
        public bool BiometricOn { get; }

        public ToggleBiometricCommand(string userId, bool biometricOn)
        {
            UserId = userId;
            BiometricOn = biometricOn;
        }
    }
}
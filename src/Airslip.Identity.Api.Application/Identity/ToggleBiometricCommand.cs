using Airslip.Common.Types.Interfaces;
using MediatR;

namespace Airslip.Identity.Api.Application.Identity
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
using Airslip.Common.Contracts;
using Airslip.Identity.MongoDb.Contracts;
using JetBrains.Annotations;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.UserProfiles
{
    [UsedImplicitly(ImplicitUseTargetFlags.Itself)]
    public class ToggleBiometricCommandHandler : IRequestHandler<ToggleBiometricCommand, IResponse>
    {
        private readonly IUserService _userService;

        public ToggleBiometricCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponse> Handle(ToggleBiometricCommand request, CancellationToken cancellationToken)
        {
            await _userService.ToggleBiometric(request.UserId, request.BiometricOn);

            return Success.Instance;
        }
    }
}
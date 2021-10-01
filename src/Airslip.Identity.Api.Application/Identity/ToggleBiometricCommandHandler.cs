﻿using Airslip.Common.Types;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using JetBrains.Annotations;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
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
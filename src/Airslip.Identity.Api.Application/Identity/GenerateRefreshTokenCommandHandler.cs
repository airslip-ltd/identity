using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class GenerateRefreshTokenCommandHandler : IRequestHandler<GenerateRefreshTokenCommand, IResponse>
    {
        private readonly IUserLifecycle _userLifecycle;

        public GenerateRefreshTokenCommandHandler(
            IUserLifecycle userLifecycle)
        {
            _userLifecycle = userLifecycle;
        }

        public async Task<IResponse> Handle(GenerateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _userLifecycle.GenerateRefreshToken(request.DeviceId, request.Token);
        }
    }
}
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class GenerateRefreshTokenCommandHandler : IRequestHandler<GenerateRefreshTokenCommand, IResponse>
    {
        private readonly IUserService _userService;

        public GenerateRefreshTokenCommandHandler(
            IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponse> Handle(GenerateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _userService.GenerateRefreshToken(request.DeviceId, request.Token);
        }
    }
}
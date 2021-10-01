using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class GenerateRefreshTokenCommandHandler : IRequestHandler<GenerateRefreshTokenCommand, IResponse>
    {
        private readonly IUserLoginService _userLoginService;

        public GenerateRefreshTokenCommandHandler(
            IUserLoginService userLoginService)
        {
            _userLoginService = userLoginService;
        }

        public async Task<IResponse> Handle(GenerateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _userLoginService.GenerateRefreshToken(request.UserId, request.DeviceId, request.Token);
        }
    }
}
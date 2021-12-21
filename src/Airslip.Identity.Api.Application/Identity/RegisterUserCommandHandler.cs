using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IResponse>
    {
        private readonly IUserService _userService;

        public RegisterUserCommandHandler(
            IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.Register(request, cancellationToken);
        }
    }
}
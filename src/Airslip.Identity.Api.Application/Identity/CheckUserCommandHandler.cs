using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts;
using Airslip.Identity.Api.Contracts.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class CheckUserCommandHandler : IRequestHandler<CheckUserCommand, IResponse>
    {
        private readonly IUserManagerService _userManagerService;
        private readonly WelcomeSettings _welcomeSettings;

        public CheckUserCommandHandler(IUserManagerService userManagerService, IOptions<WelcomeSettings> welcomeOptions)
        {
            _userManagerService = userManagerService;
            _welcomeSettings = welcomeOptions.Value;
        }
        
        public async Task<IResponse> Handle(CheckUserCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser? applicationUser = await _userManagerService.FindByEmail(request.Email);

            return applicationUser is null
                ? new UserResponse(_welcomeSettings.NewUser.Message, true)
                : new UserResponse(_welcomeSettings.ExistingUser.Message, false);
        }
    }
}
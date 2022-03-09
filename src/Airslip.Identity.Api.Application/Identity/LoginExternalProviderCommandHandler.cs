using Airslip.Common.Auth.Data;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class LoginExternalProviderCommandHandler : IRequestHandler<LoginExternalProviderCommand, IResponse>
    {
        private readonly IUserLifecycle _userLifecycle;
        private readonly IIdentityContext _context;
        private readonly ILogger _logger;

        public LoginExternalProviderCommandHandler(
            IUserLifecycle userLifecycle,
            IIdentityContext context)
        {
            _userLifecycle = userLifecycle;
            _context = context;
            _logger = Log.Logger;
        }

        public async Task<IResponse> Handle(LoginExternalProviderCommand request, CancellationToken cancellationToken)
        {
            User? user = await _context.GetByEmail(request.Email);
            bool isNewUser = user is null;

            user = isNewUser 
                ? await _context.AddEntity(new User(request.Email, request.FirstName, request.LastName, UserRoles.User))
                : await _context.GetEntity<User>(user!.Id);
            
            return await _userLifecycle.GenerateUserResponse(user!, isNewUser, request.DeviceId);
        }
    }
}
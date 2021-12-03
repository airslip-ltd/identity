using Airslip.Common.Types.Interfaces;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts;
using Airslip.Identity.Api.Contracts.Entities;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class GenerateJwtBearerTokenCommandHandler : IRequestHandler<LoginUserCommand, IResponse>
    {
        private readonly IUserService _userService;
        private readonly IIdentityContext _context;
        private readonly IUserManagerService _userManagerService;
        private readonly ILogger _logger;

        public GenerateJwtBearerTokenCommandHandler(
            IUserService userService,
            IIdentityContext context,
            IUserManagerService userManagerService)
        {
            _userService = userService;
            _context = context;
            _userManagerService = userManagerService;
            _logger = Log.Logger;
        }

        public async Task<IResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser? applicationUser = await _userManagerService.FindByEmail(request.Email);

            if (applicationUser is null)
                return new NotFoundResponse(nameof(request.Email), request.Email, "New user");

            bool canLogin = await _userManagerService.TryToLogin(applicationUser, request.Password);
            
            if (!canLogin)
                return new IncorrectPasswordResponse("You have entered an incorrect password.");

            User? user = await _context.GetByEmail(request.Email);
            if (user == null)
                return new NotFoundResponse(nameof(request.Email), request.Email, "Unable to find user");

            user = (await _context.GetEntity<User>(user.Id))!;

            string? yapilyUserId = user.GetOpenBankingProviderId("Yapily");

            if (yapilyUserId is null)
                return new InvalidResource("YapilyUserId", "Doesn't exist");

            _logger.Information("User {UserId} successfully logged in", user.Id);

            return await _userService.GenerateUserResponse(user, false, yapilyUserId, request.DeviceId);
        }
    }
}
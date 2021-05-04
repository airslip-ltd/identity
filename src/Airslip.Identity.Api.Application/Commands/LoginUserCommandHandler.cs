using Airslip.Common.Contracts;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Security.Jwt;
using MediatR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Commands
{
    public class GenerateJwtBearerTokenCommandHandler : IRequestHandler<LoginUserCommand, IResponse>
    {
        private readonly IUserService _userService;
        private readonly IUserManagerService _userManagerService;
        private readonly JwtSettings _jwtSettings;

        public GenerateJwtBearerTokenCommandHandler(
            IUserService userService,
            IOptions<JwtSettings> jwtSettingsOptions,
            IUserManagerService userManagerService)
        {
            _userService = userService;
            _userManagerService = userManagerService;
            _jwtSettings = jwtSettingsOptions.Value;
        }

        public async Task<IResponse> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            bool canLogin = await _userManagerService.TryToLogin(command.Email, command.Password);

            if (!canLogin)
            {
                User? possibleUser = await _userService.GetByEmail(command.Email);
                return possibleUser == null
                    ? new NotFoundResponse(
                        nameof(command.Email), 
                        command.Email, 
                        "A user with this email doesn't exist")
                    : new ErrorResponse("INCORRECT_PASSWORD", "Password is incorrect");
            }

            User? user = await _userService.GetByEmail(command.Email);

            if (user == null)
                return new InvalidResource(nameof(User), "Unable to find user");
            
            string jwtBearerToken = JwtBearerToken.Generate(
                _jwtSettings.Key,
                _jwtSettings.Audience,
                _jwtSettings.Issuer,
                _jwtSettings.ExpiresTime,
                user.Id);

            bool hasAddedInstitution = user.Institutions.Count > 0;
            
            return new AuthenticatedUserResponse(jwtBearerToken, hasAddedInstitution);
        }
    }
}
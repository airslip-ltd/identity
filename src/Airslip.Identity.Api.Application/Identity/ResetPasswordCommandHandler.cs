using Airslip.Common.Contracts;
using Airslip.Common.Types.Failures;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Identity.MongoDb.Contracts.Identity;
using Airslip.Identity.MongoDb.Contracts.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, IResponse>
    {
        private readonly IUserManagerService _userManagerService;

        public ResetPasswordCommandHandler(IUserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }

        public async Task<IResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await _userManagerService.FindByEmail(request.Email);

            if (user is null)
                return new NotFoundResponse(nameof(request.Email), request.Email, "Unable to find user");

            if (!request.Password.Equals(request.ConfirmPassword))
                return new InvalidAttribute(nameof(request.Password), request.Password, "Passwords do not match");

            IdentityResult resetPassResult = await _userManagerService.ResetPassword(
                user, 
                request.Token, 
                request.Password);

            if (resetPassResult.Succeeded) 
                return Success.Instance;
            
            List<ErrorResponse> errors = resetPassResult.Errors
                .Select(identityError => new ErrorResponse(
                    identityError.Code,
                    identityError.Description))
                .ToList();

            return new ErrorResponses(errors);
        }
    }
}
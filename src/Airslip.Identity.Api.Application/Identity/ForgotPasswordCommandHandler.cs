using Airslip.Common.Contracts;
using Airslip.Common.Types;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Contracts;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Identity.MongoDb.Contracts.Identity;
using MediatR;
using Microsoft.Extensions.Options;
using Serilog;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, IResponse>
    {
        private readonly IEmailSender _emailSender;
        private readonly IUserManagerService _userManagerService;
        private readonly PublicApiSettings _publicApiSettings;
        private readonly ILogger _logger;

        public ForgotPasswordCommandHandler(
            IUserManagerService userManagerService,
            IEmailSender emailSender,
            IOptions<PublicApiSettings> publicApiOptions)
        {
            _userManagerService = userManagerService;
            _emailSender = emailSender;
            _publicApiSettings = publicApiOptions.Value;
            _logger = Log.Logger;
        }

        public async Task<IResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await _userManagerService.FindByEmail(request.Email);

            if (user is null)
                return new NotFoundResponse(nameof(request.Email), request.Email, "Unable to find user");

            string token = await _userManagerService.GeneratePasswordResetToken(user);

            string resetPasswordUrl = ForgotPasswordEmailConstants.GetPasswordResetUrl(
                _publicApiSettings.Base.BaseUri, 
                request.RelativeEndpoint,
                token, 
                user.Email);

            EmailOutcome outcome = await _emailSender.SendEmail(
                new List<EmailAddressRecipient> { new("tjmcdonough20@gmail.com", "tjmcdonough20@gmail.com") },
                ForgotPasswordEmailConstants.Subject,
                ForgotPasswordEmailConstants.GetPlainTextContent(resetPasswordUrl),
                string.Empty);

            if(outcome.Success)
                _logger.Error(outcome.ErrorReason);

            return new ForgotPasswordResponse(resetPasswordUrl);
        }
    }
}
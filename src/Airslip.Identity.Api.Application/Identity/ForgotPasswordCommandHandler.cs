using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Identity
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, IResponse>
    {
        private readonly IEmailNotificationService _emailNotificationService;

        public ForgotPasswordCommandHandler(
            IEmailNotificationService emailNotificationService)
        {
            _emailNotificationService = emailNotificationService;
        }

        public Task<IResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            return _emailNotificationService.SendPasswordReset(request.Email, request.RelativeEndpoint);
        }
    }
}
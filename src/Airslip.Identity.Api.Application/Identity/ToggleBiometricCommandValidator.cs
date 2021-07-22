using Airslip.Common.Types.Validator;
using FluentValidation;

namespace Airslip.Identity.Api.Application.Identity
{
    public class ToggleBiometricCommandValidator : AbstractValidator<ToggleBiometricCommand>
    {
        public ToggleBiometricCommandValidator()
        {
            RuleFor(command => command.UserId).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(RequiredConstants.Message)
                .WithState(command => new
                {
                    Attribute = nameof(command.UserId),
                    Value = command.UserId
                })
                .WithErrorCode(RequiredConstants.ErrorCode);
        }
    }
}
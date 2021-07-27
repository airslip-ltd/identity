using Airslip.Common.Types.Validator;
using FluentValidation;

namespace Airslip.Identity.Api.Application.Identity
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(command => command).SetValidator(new AuthenticateCommandValidator());
            
            RuleFor(command => command.ReferenceId).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(RequiredConstants.Message)
                .WithState(command => new
                {
                    Attribute = nameof(command.ReferenceId),
                    Value = command.ReferenceId
                })
                .WithErrorCode(RequiredConstants.ErrorCode);
        }
    }
}
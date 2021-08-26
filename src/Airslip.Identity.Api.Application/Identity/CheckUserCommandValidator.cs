using FluentValidation;

namespace Airslip.Identity.Api.Application.Identity
{
    public class CheckUserCommandValidator : AbstractValidator<CheckUserCommand>
    {
        public CheckUserCommandValidator()
        {
            RuleFor(command => command).SetValidator(new AuthenticateCommandValidator());
        }
    }
}
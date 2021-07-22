using FluentValidation;

namespace Airslip.Identity.Api.Application.Identity
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(command => command).SetValidator(new AuthenticateCommandValidator());
        }
    }
}
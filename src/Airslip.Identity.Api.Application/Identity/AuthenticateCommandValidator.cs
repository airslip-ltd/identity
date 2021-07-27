using Airslip.Common.Types.Extensions;
using Airslip.Common.Types.Validator;
using FluentValidation;

namespace Airslip.Identity.Api.Application.Identity
{
    public class AuthenticateCommandValidator : AbstractValidator<IAuthenticateRequest>
    {
        public AuthenticateCommandValidator()
        {
            RuleFor(command => command.Email).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(RequiredConstants.Message)
                .WithState(command => new
                {
                    Attribute = nameof(command.Email),
                    Value = command.Email
                })
                .WithErrorCode(RequiredConstants.ErrorCode)
                .Must(s => s.IsValidEmail())
                .WithMessage(InvalidConstants.Message)
                .WithState(command => new
                {
                    Attribute = nameof(command.Email),
                    Value = command.Email,
                    Validation = InvalidConstants.MustBeValidEmail
                })
                .WithErrorCode(InvalidConstants.ErrorCode);
        }
    }
}
using Airslip.Common.Types.Validator;
using FluentValidation;

namespace Airslip.Identity.Api.Application.Identity
{
    public class GenerateRefreshTokenCommandValidator : AbstractValidator<GenerateRefreshTokenCommand>
    {
        public GenerateRefreshTokenCommandValidator()
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
            
            RuleFor(command => command.Token).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(RequiredConstants.Message)
                .WithState(command => new
                {
                    Attribute = nameof(command.Token),
                    Value = command.Token
                })
                .WithErrorCode(RequiredConstants.ErrorCode);
        }
    }
}
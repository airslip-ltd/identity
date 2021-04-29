﻿using Airslip.Common.Types.Extensions;
using Airslip.Common.Types.Validator;
using FluentValidation;

namespace Airslip.Identity.Api.Application.Commands
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(command => command.Email).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(RequiredConstants.Message)
                .WithState(x => new
                {
                    Attribute = nameof(x.Email),
                    Value = x.Email
                })
                .WithErrorCode(RequiredConstants.ErrorCode)
                .Must(s => s.IsValidEmail())
                .WithMessage(InvalidConstants.Message)
                .WithState(x => new
                {
                    Attribute = nameof(x.Email),
                    Value = x.Email,
                    Validation = InvalidConstants.MustBeValidEmail
                })
                .WithErrorCode(InvalidConstants.ErrorCode);
        }
    }
}
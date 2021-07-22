using Airslip.Common.Types.Validator;
using FluentValidation;
using JetBrains.Annotations;

namespace Airslip.Identity.Api.Application.UserProfiles
{
    [UsedImplicitly(ImplicitUseTargetFlags.Itself)]
    public class UpdateUserProfilePhotoCommandValidator : AbstractValidator<UpdateUserProfilePhotoCommand>
    {
        public UpdateUserProfilePhotoCommandValidator()
        {
            RuleFor(command => command.Photo).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(RequiredConstants.Message)
                .WithState(command => new
                {
                    Attribute = nameof(command.Photo),
                    Value = command.Photo
                })
                .WithErrorCode(RequiredConstants.ErrorCode);

            RuleFor(command => command.Photo!.FileName).Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(RequiredConstants.Message)
                .WithState(command => new
                {
                    Attribute = nameof(command.Photo.Name),
                    Value = command.Photo?.Name
                })
                .WithErrorCode(RequiredConstants.ErrorCode);

            RuleFor(command => command.Photo!.FileName).Cascade(CascadeMode.Stop)
                .Must(s => s.ToLowerInvariant().EndsWith("png") || s.ToLowerInvariant().EndsWith("jpeg") ||
                           s.ToLowerInvariant().EndsWith("jpg") || s.ToLowerInvariant().EndsWith("heic"))
                .WithMessage(RequiredConstants.Message)
                .WithState(command => new
                {
                    Attribute = nameof(command.Photo.Name),
                    Value = command.Photo?.Name
                })
                .WithErrorCode(RequiredConstants.ErrorCode);

            RuleFor(command => command.Photo!.Length).Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .WithMessage(RequiredConstants.Message)
                .WithState(command => new
                {
                    Attribute = nameof(command.Photo.Length),
                    Value = command.Photo?.Length
                })
                .WithErrorCode(RequiredConstants.ErrorCode);
        }
    }
}
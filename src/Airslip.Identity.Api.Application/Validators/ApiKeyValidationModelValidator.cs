using Airslip.Common.Auth.Enums;
using Airslip.Identity.Api.Contracts.Models;
using FluentValidation;

namespace Airslip.Identity.Api.Application.Validators
{
    public class ApiKeyValidationModelValidator : AbstractValidator<ApiKeyValidationModel>
    {
        public ApiKeyValidationModelValidator()
        {
            RuleFor(o => o.ApiKey)
                .NotEmpty();
            
            RuleFor(o => o.EntityId)
                .NotEmpty();
            
            RuleFor(o => o.AirslipUserType)
                .NotEmpty();

            RuleFor(o => o.AirslipUserType)
                .Equal(AirslipUserType.Merchant);
            
            RuleFor(o => o.VerificationToken)
                .NotEmpty();
        }
    }
}
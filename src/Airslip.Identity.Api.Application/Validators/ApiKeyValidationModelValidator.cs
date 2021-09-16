using Airslip.Common.Auth.Enums;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Models;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

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
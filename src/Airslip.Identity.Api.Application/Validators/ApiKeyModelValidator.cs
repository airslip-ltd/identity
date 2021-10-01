using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Models;
using Airslip.Common.Types.Enums;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace Airslip.Identity.Api.Application.Validators
{
    /// <summary>
    /// IModelValidator implementation for validating a new or updated Api Key entry
    /// </summary>
    public class ApiKeyModelValidator : AbstractValidator<ApiKeyModel>, IModelValidator<ApiKeyModel>
    {
        private const int NameMaxLength = 50;

        public ApiKeyModelValidator()
        {
            RuleFor(o => o.Name)
                .Length(1, NameMaxLength);
            RuleFor(o => o.Name)
                .NotEmpty();
            
            RuleFor(o => o.KeyValue)
                .NotEmpty();
            
            RuleFor(o => o.EntityId)
                .NotEmpty();

            RuleFor(o => o.AirslipUserType)
                .Equal(AirslipUserType.Merchant);

        }
        
        public async Task<ValidationResultModel> ValidateAdd(ApiKeyModel model)
        {
            ValidationResult? validationResult = await ValidateAsync(model);

            ValidationResultModel result = new();
            validationResult.Errors
                .ForEach(o => result.AddMessage(o.PropertyName, o.ErrorMessage));
            
            return await Task.FromResult(result);
        }

        public Task<ValidationResultModel> ValidateUpdate(ApiKeyModel model) 
            => ValidateAdd(model);
    }
}
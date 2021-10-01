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
    public class QrCodeModelValidator : AbstractValidator<QrCodeModel>, IModelValidator<QrCodeModel>
    {
        private const int NameMaxLength = 50;

        public QrCodeModelValidator()
        {
            RuleFor(o => o.Name)
                .Length(1, NameMaxLength);
            RuleFor(o => o.Name)
                .NotEmpty();
            
            RuleFor(o => o.StoreId)
                .NotEmpty();
            
            RuleFor(o => o.CheckoutId)
                .NotEmpty();
            
            RuleFor(o => o.EntityId)
                .NotEmpty();

            RuleFor(o => o.AirslipUserType)
                .Equal(AirslipUserType.Merchant);
        }
        
        public async Task<ValidationResultModel> ValidateAdd(QrCodeModel model)
        {
            ValidationResult? validationResult = await ValidateAsync(model);

            ValidationResultModel result = new();
            validationResult.Errors
                .ForEach(o => result.AddMessage(o.PropertyName, o.ErrorMessage));
            
            return await Task.FromResult(result);
        }

        public Task<ValidationResultModel> ValidateUpdate(QrCodeModel model) 
            => ValidateAdd(model);
    }
}
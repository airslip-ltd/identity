using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Common.Repository.Types.Models;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace Airslip.Identity.Api.Application.Validators
{
    /// <summary>
    /// IModelValidator implementation for validating a new or updated Api Key entry
    /// </summary>
    public class UserModelValidator : AbstractValidator<UserModel>, IModelValidator<UserModel>
    {
        public async Task<ValidationResultModel> ValidateAdd(UserModel model)
        {
            ValidationResult? validationResult = await ValidateAsync(model);

            ValidationResultModel result = new();
            validationResult.Errors
                .ForEach(o => result.AddMessage(o.PropertyName, o.ErrorMessage));
            
            return await Task.FromResult(result);
        }

        public Task<ValidationResultModel> ValidateUpdate(UserModel model) 
            => ValidateAdd(model);
    }
}
using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Common.Repository.Types.Models;
using Airslip.Identity.Api.Contracts.Models;
using FluentValidation;
using FluentValidation.Results;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Validators;

/// <summary>
/// IModelValidator implementation for validating a new or updated Api Key entry
/// </summary>
public class ProfileModelValidator : AbstractValidator<ProfileModel>, IModelValidator<ProfileModel>
{
    public async Task<ValidationResultModel> ValidateAdd(ProfileModel model)
    {
        ValidationResult? validationResult = await ValidateAsync(model);

        ValidationResultModel result = new();
        validationResult.Errors
            .ForEach(o => result.AddMessage(o.PropertyName, o.ErrorMessage));
            
        return await Task.FromResult(result);
    }

    public Task<ValidationResultModel> ValidateUpdate(ProfileModel model) 
        => ValidateAdd(model);
}
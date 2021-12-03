using Airslip.Common.Auth.Data;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Models;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Application.Validators
{
    /// <summary>
    /// IModelValidator implementation for validating a new or updated Api Key entry
    /// </summary>
    public class UserRoleUpdateModelValidator : AbstractValidator<UserRoleUpdateModel>, IModelValidator<UserRoleUpdateModel>
    {
        private static readonly List<string> roles = new()
        {
            UserRoles.Administrator,
            UserRoles.Manager,
            UserRoles.User
        };
        public UserRoleUpdateModelValidator()
        {
            RuleFor(o => o.UserRole)
                .NotEmpty();
            RuleFor(o => o.UserRole)
                .Must(s => roles.Contains(s ?? string.Empty))
                .WithMessage("Invalid UserRole specified");
        }
        
        public async Task<ValidationResultModel> ValidateAdd(UserRoleUpdateModel model)
        {
            ValidationResult? validationResult = await ValidateAsync(model);

            ValidationResultModel result = new();
            validationResult.Errors
                .ForEach(o => result.AddMessage(o.PropertyName, o.ErrorMessage));
            
            return await Task.FromResult(result);
        }

        public Task<ValidationResultModel> ValidateUpdate(UserRoleUpdateModel model) 
            => ValidateAdd(model);
    }
}
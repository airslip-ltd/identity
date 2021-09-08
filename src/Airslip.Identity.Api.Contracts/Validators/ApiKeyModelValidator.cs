using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Models;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Contracts.Validators
{
    /// <summary>
    /// IValildator implementation for validating a new or updated Api Key entry
    /// </summary>
    public class ApiKeyModelValidator : IValidator<ApiKeyModel>
    {
        private const int NameMaxLength = 50;
 
        public async Task<ValidationResultModel> ValidateAdd(ApiKeyModel model)
        {
            var result = new ValidationResultModel();
            
            // Simple validation of string length and such like, assumes 
            //  we are not using standard MVC model validation

            if (model.Name == null)
            {
                // Failed as it is required
                result.AddMessage("Name", $"Value is required");
            }
            
            if (model.Name?.Length > NameMaxLength)
            {
                // Failed as it is too long
                result.AddMessage("Name", $"Maximum length of {NameMaxLength}");
            }
            
            return await Task.FromResult(result);
        }

        public async Task<ValidationResultModel> ValidateUpdate(ApiKeyModel model)
        {
            return await ValidateAdd(model);
        }
    }
}
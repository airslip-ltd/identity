using Airslip.Common.Repository.Models;
using System.Threading.Tasks;

namespace Airslip.Common.Repository.Interfaces
{
    /// <summary>
    /// A validation definition, this is required for every repository implementation
    ///  as it tells the repository how to validate the record
    /// </summary>
    /// <typeparam name="TModel">The model tyoe to validate</typeparam>
    public interface IModelValidator<TModel> where TModel : class, IModel
    {
        /// <summary>
        /// Validate on adding a record
        /// </summary>
        /// <param name="model">The model to validate</param>
        /// <returns>A validation result model containing any messages and valid indicator</returns>
        Task<ValidationResultModel> ValidateAdd(TModel model);
        
        /// <summary>
        /// Validate on updating a record
        /// </summary>
        /// <param name="model">The model to validate</param>
        /// <returns>A validation result model containing any messages and valid indicator</returns>
        Task<ValidationResultModel> ValidateUpdate(TModel model);
    }
}
using Airslip.Common.Repository.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airslip.Common.Repository.Interfaces
{
    /// <summary>
    /// A generic search definition for use when searching entities
    /// </summary>
    /// <typeparam name="TEntity">The entity type we are searching</typeparam>
    /// <typeparam name="TModel">The model type we are returning</typeparam>
    public interface IEntitySearch<TEntity, TModel>
        where TEntity : class, IEntity
        where TModel : class, IModel
    {
        /// <summary>
        /// A query to get search results based on a set of search filters
        /// </summary>
        /// <param name="searchFilters">A set of search filters</param>
        /// <returns>A list of formatted models</returns>
        Task<List<TModel>> GetSearchResults(List<SearchFilterModel> searchFilters);
    }
}

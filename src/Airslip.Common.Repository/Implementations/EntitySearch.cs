using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airslip.Common.Repository.Implementations
{
    /// <summary>
    /// Generic implementation of entity search, designed to allow for quickly creating new APIs with search capability
    /// </summary>
    /// <typeparam name="TEntity">The entity type we will be searching for</typeparam>
    /// <typeparam name="TModel">The model type we will be returning</typeparam>
    public class EntitySearch<TEntity, TModel> : IEntitySearch<TEntity, TModel> 
        where TEntity : class, IEntity 
        where TModel : class, IModel
    {
        private readonly IContext _context;
        private readonly IModelMapper<TModel> _mapper;
        private readonly IEntitySearchFormatter<TModel>? _searchFormatter;
        
        public EntitySearch(IContext context, IModelMapper<TModel> mapper, IEntitySearchFormatter<TModel>? searchFormatter)
        {
            _context = context;
            _mapper = mapper;
            _searchFormatter = searchFormatter;
        }
        
        /// <summary>
        /// Singe function that takes a list of search filters and returns a list of formatted models
        /// </summary>
        /// <param name="searchFilters">The search filters we wish to filter by</param>
        /// <returns>A list of formatted models</returns>
        public async Task<List<TModel>> GetSearchResults(List<SearchFilterModel> searchFilters)
        {
            // Get search results for our entities
            List<TEntity> searchResults = await _context.GetEntities<TEntity>(searchFilters);
            List<TModel> results = new();
            
            // Format them into models
            foreach (TEntity result in searchResults)
            {
                // Create a new model using the mapper
                TModel newModel = _mapper.Create(result);

                // If we have a search formatter we can use it here to populate any additional data
                if (_searchFormatter != null)
                {
                    newModel = await _searchFormatter.FormatModel(newModel);                    
                }

                // Add to the list
                results.Add(newModel);
            }
            
            return results;
        }
    }
}
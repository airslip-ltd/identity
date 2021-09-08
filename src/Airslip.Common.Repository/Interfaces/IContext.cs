using Airslip.Common.Repository.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Common.Repository.Interfaces
{
    /// <summary>
    /// An interface defining the expected data access functions.
    ///  By defining it here we remove any specific requirements pertaining to a
    ///  particular context type, meaning a context could be anything
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Add an entity to the context, assumes all validation has already passed
        /// </summary>
        /// <param name="newEntity">The new entity to add</param>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <returns>The entity after being added to the context</returns>
        Task<TEntity> AddEntity<TEntity>(TEntity newEntity) 
            where TEntity : class, IEntity;

        /// <summary>
        /// Saves and changes to the context
        /// </summary>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>An int representing the number of changes</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default (CancellationToken));
        
        /// <summary>
        /// Gets a particular entity based on an Id
        /// </summary>
        /// <param name="id">The Id of the entity we wish to load</param>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <returns>The loaded entity, would be null if not found</returns>
        Task<TEntity?> GetEntity<TEntity>(string id)
            where TEntity : class, IEntity;

        /// <summary>
        /// Returns a list of entities based on search criteria
        /// </summary>
        /// <param name="searchFilters">Search filters to apply to the search</param>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <returns>A list of entities matching the search criteria</returns>
        Task<List<TEntity>> GetEntities<TEntity>(List<SearchFilterModel> searchFilters)
            where TEntity : class, IEntity;

        /// <summary>
        /// Returns a list of entities based on search criteria
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <returns>A list of entities matching the search criteria</returns>
        IQueryable<TEntity> QueryableOf<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Returns a list of entities based on search criteria
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <returns>A list of entities matching the search criteria</returns>
        Task<List<TEntity>> ExecuteAsync<TEntity>(IQueryable<TEntity> queryable)
            where TEntity : class;
    }
}
namespace Airslip.Common.Repository.Interfaces
{
    /// <summary>
    /// Simple model mapping interface for creating and updating entities and models
    /// </summary>
    /// <typeparam name="TModel">The type of model we want to manage using this interface</typeparam>
    public interface IModelMapper<TModel>
    {
        /// <summary>
        /// Create an entity from a particular source
        /// </summary>
        /// <param name="source">The source object</param>
        /// <typeparam name="TEntity">The destination entity type</typeparam>
        /// <returns>A new entity mapped using the data contained in the source</returns>
        TEntity CreateEntity<TEntity>(TModel source);
        
        /// <summary>
        /// Updated an already created entity with data from a particular source
        /// </summary>
        /// <param name="source">The source object</param>
        /// <param name="destination">The destination entity</param>
        /// <typeparam name="TEntity">The destination entity type</typeparam>
        /// <returns>The updated entity mapped using the data contained in the source</returns>
        TEntity UpdateEntity<TEntity>(TModel source, TEntity destination);
        
        /// <summary>
        /// Create a model from a particular source
        /// </summary>
        /// <param name="source">The source object</param>
        /// <typeparam name="TEntity">The source entity type</typeparam>
        /// <returns>A new model mapped using the data contained in the source</returns>
        TModel CreateModel<TEntity>(TEntity source);
    }
}
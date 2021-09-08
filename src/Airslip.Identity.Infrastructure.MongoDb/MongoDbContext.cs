using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Infrastructure.MongoDb
{
    public class MongoDbContext : IContext
    {
        private readonly AirslipMongoDbContext _mongoDbContext;

        public MongoDbContext(AirslipMongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }
        
        public async Task<TEntity> AddEntity<TEntity>(TEntity newEntity) 
            where TEntity : class, IEntityWithId
        {
            // Find appropriate collection
            IMongoCollection<TEntity> collection = _mongoDbContext.CollectionByType<TEntity>();
            
            // Add entity to collection
            await collection.InsertOneAsync(newEntity);
            
            // Return the added entity - likely to be the same object
            return newEntity;
        }

        public async Task<TEntity?> GetEntity<TEntity>(string id) 
            where TEntity : class, IEntityWithId
        {
            // Find appropriate collection
            IMongoCollection<TEntity> collection = _mongoDbContext.CollectionByType<TEntity>();

            return await collection.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        public async Task<TEntity> UpdateEntity<TEntity>(TEntity updatedEntity) where TEntity : class, IEntityWithId
        {
            IMongoCollection<TEntity> collection = _mongoDbContext.CollectionByType<TEntity>();
            
            await collection.ReplaceOneAsync(user => user.Id == updatedEntity.Id, updatedEntity);

            return updatedEntity;
        }

        public Task<List<TEntity>> GetEntities<TEntity>(List<SearchFilterModel> searchFilters) 
            where TEntity : class, IEntityWithId
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<TEntity> QueryableOf<TEntity>() 
            where TEntity : class
        {
            IMongoCollection<TEntity> collection = _mongoDbContext.CollectionByType<TEntity>();

            return collection.AsQueryable();
        }
    }
}
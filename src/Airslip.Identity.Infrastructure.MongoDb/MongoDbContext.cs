using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Infrastructure.MongoDb
{
    public class MongoDbContext : IContext
    {
        public Task<TEntity> AddEntity<TEntity>(TEntity newEntity) 
            where TEntity : class, IEntity
        {
            throw new System.NotImplementedException();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<TEntity?> GetEntity<TEntity>(string id) 
            where TEntity : class, IEntity
        {
            throw new System.NotImplementedException();
        }

        public Task<List<TEntity>> GetEntities<TEntity>(List<SearchFilterModel> searchFilters) 
            where TEntity : class, IEntity
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<TEntity> QueryableOf<TEntity>() 
            where TEntity : class
        {
            throw new System.NotImplementedException();
        }

        public Task<List<TEntity>> ExecuteAsync<TEntity>(IQueryable<TEntity> queryable) 
            where TEntity : class
        {
            throw new System.NotImplementedException();
        }
    }
}
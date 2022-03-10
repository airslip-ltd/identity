using Airslip.Common.Repository.Types.Enums;
using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Common.Repository.Types.Models;
using Airslip.Common.Services.MongoDb;
using Airslip.Common.Services.MongoDb.Extensions;
using Airslip.Common.Types.Configuration;
using Airslip.Common.Types.Enums;
using Airslip.Common.Types.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Services.MongoDb.Implementations
{
    public class SearchContext : ISearchContext
    {
        protected readonly IMongoDatabase Database;

        public SearchContext(MongoClient mongoClient,   
            IOptions<MongoDbSettings> options)
        {
            Database = mongoClient.GetDatabase(options.Value.DatabaseName);
        }

        public async Task<EntitySearchResult<TEntity>> SearchEntities<TEntity>(EntitySearchQueryModel entitySearch, 
            List<SearchFilterModel> mandatoryFilters) 
            where TEntity : class, IEntityWithId
        {
            IMongoCollection<TEntity> collection = Database.CollectionByType<TEntity>();

            SortDefinition<TEntity> sortDefinition = entitySearch.BuildSortDefinition<TEntity>();
            
            FilterDefinitionBuilder<TEntity>? filterBuilder = Builders<TEntity>.Filter;
            FilterDefinition<TEntity> userFilter = entitySearch.BuildFilterDefinition<TEntity>();
            FilterDefinition<TEntity> mandatoryFilter = mandatoryFilters.BuildFilterDefinition<TEntity>();

            FilterDefinition<TEntity> bothFilters = filterBuilder.And(userFilter, mandatoryFilter);
            
            (IReadOnlyList<TEntity> data, int totalCount) mySearch = await collection
                .AggregateByPage(bothFilters, sortDefinition,
                entitySearch.Page * entitySearch.RecordsPerPage, entitySearch.RecordsPerPage, CancellationToken.None);

            return new EntitySearchResult<TEntity>(
                mySearch.data.ToList(),
                mySearch.totalCount
                );
        }

        public Task<EntitySearchResult<TEntity>> SearchEntities<TEntity>(IQueryable<TEntity> baseQuery, EntitySearchQueryModel entitySearch, List<SearchFilterModel> mandatoryFilters) where TEntity : class, IEntityWithId
        {
            throw new System.NotImplementedException();
        }

        public Task<int> RecordCount<TEntity>(EntitySearchQueryModel entitySearch, List<SearchFilterModel> mandatoryFilters) where TEntity : class, IEntityWithId
        {
            throw new System.NotImplementedException();
        }
    }
}
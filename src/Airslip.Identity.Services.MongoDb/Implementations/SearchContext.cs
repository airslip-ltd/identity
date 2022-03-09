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
        
        // public Task<List<TEntity>> SearchEntities<TEntity>(List<SearchFilterModel> searchFilters)
        //     where TEntity : class, IEntityWithId
        // {
        //     if (typeof(IEntityWithOwnership).IsAssignableFrom(typeof(TEntity)))
        //     {
        //         switch (_userContext.AirslipUserType ?? AirslipUserType.Standard)
        //         {
        //             case AirslipUserType.Standard:
        //                 searchFilters.Add(new SearchFilterModel("userId", _userContext.UserId!));
        //                 break;
        //             default:
        //                 searchFilters.Add(new SearchFilterModel("entityId", 
        //                     _userContext.EntityId!));
        //                 searchFilters.Add(new SearchFilterModel("airslipUserType", 
        //                     _userContext.AirslipUserType!));
        //                 break;
        //         } 
        //     }
        //
        //     FilterDefinitionBuilder<TEntity>? filterBuilder = Builders<TEntity>.Filter;
        //     List<FilterDefinition<TEntity>> filters = new();
        //     foreach (SearchFilterModel searchFilterModel in searchFilters)
        //     {
        //         switch (searchFilterModel.Value)
        //         {
        //             case bool boolValue:
        //                 filters.Add(filterBuilder.Eq(searchFilterModel.ColumnField, boolValue));
        //                 break;
        //             case int intValue:
        //                 filters.Add(filterBuilder.Eq(searchFilterModel.ColumnField, intValue));
        //                 break;
        //             case long lngValue:
        //                 filters.Add(filterBuilder.Eq(searchFilterModel.ColumnField, lngValue));
        //                 break;
        //             case AirslipUserType airslipUserType:
        //                 filters.Add(filterBuilder.Eq(searchFilterModel.ColumnField, airslipUserType));
        //                 break;
        //             default:
        //                 filters.Add(filterBuilder.Eq(searchFilterModel.ColumnField, searchFilterModel.Value
        //                     .ToString()));
        //                 break;
        //         }
        //     }
        //
        //     IMongoCollection<TEntity> collection = Database.CollectionByType<TEntity>();
        //
        //     return collection
        //         .Find(filters.Count > 0 ? filterBuilder.And(filters) : FilterDefinition<TEntity>.Empty)
        //         .ToListAsync();
        // }

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
            
            (IReadOnlyList<TEntity> data, int totalPages) mySearch = await collection
                .AggregateByPage(bothFilters, sortDefinition,
                entitySearch.Page * entitySearch.RecordsPerPage, entitySearch.RecordsPerPage, CancellationToken.None);

            return new EntitySearchResult<TEntity>(
                mySearch.data.ToList(),
                5
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
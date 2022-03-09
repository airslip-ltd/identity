using Airslip.Common.Repository.Types.Constants;
using Airslip.Common.Repository.Types.Enums;
using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Common.Repository.Types.Models;
using Airslip.Common.Types.Enums;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Airslip.Identity.Services.MongoDb.Implementations;

public static class Extensions
{
    public static SortDefinition<TEntity> BuildSortDefinition<TEntity>(this EntitySearchQueryModel entitySearch)
    {
        SortDefinition<TEntity>? sortDefinition = null;

        foreach (EntitySearchSortModel sort in entitySearch.Sort)
        {
            Func<FieldDefinition<TEntity>, SortDefinition<TEntity>> predicate;
            switch (sort.Sort)
            {
                case SortOrder.Desc:
                    predicate = sortDefinition == null ? Builders<TEntity>.Sort.Descending : sortDefinition.Descending;
                    break;
                default:
                    predicate = sortDefinition == null ? Builders<TEntity>.Sort.Ascending : sortDefinition.Ascending;
                    break;
            }
                
            sortDefinition = predicate(sort.Field);
        }
        
        return sortDefinition ?? Builders<TEntity>.Sort.Descending(nameof(IEntityWithId.Id));
    }
    
    private static dynamic CreateExpression<TEntity>(this SearchFilterModel searchFilterModel, FilterDefinitionBuilder<TEntity> filterBuilder)
    {
        dynamic filter;
            
        switch (searchFilterModel.OperatorValue)
        {
            case Operators.OPERATOR_CONTAINS:
                filter = filterBuilder.Regex(searchFilterModel.ColumnField, 
                    new BsonRegularExpression(searchFilterModel.Value));
                break;
            case Operators.OPERATOR_EQUALS:
                filter = filterBuilder.Eq(searchFilterModel.ColumnField, searchFilterModel.Value);
                break;
            case Operators.OPERATOR_GREATER_THAN_EQUALS:
                filter = filterBuilder.Gte(searchFilterModel.ColumnField, searchFilterModel.Value);
                break;
            case Operators.OPERATOR_LESS_THAN_EQUALS:
                filter = filterBuilder.Lte(searchFilterModel.ColumnField, searchFilterModel.Value);
                break;
            case Operators.OPERATOR_GREATER_THAN:
                filter = filterBuilder.Gt(searchFilterModel.ColumnField, searchFilterModel.Value);
                break;
            case Operators.OPERATOR_LESS_THAN:
                filter = filterBuilder.Lt(searchFilterModel.ColumnField, searchFilterModel.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return filter;
    }
    
    public static FilterDefinition<TEntity> BuildFilterDefinition<TEntity>(this EntitySearchQueryModel entitySearch)
    {
        if (entitySearch.Search == null || (entitySearch.Search?.Items.Count ?? 0) == 0) 
            return FilterDefinition<TEntity>.Empty;
        
        FilterDefinitionBuilder<TEntity>? filterBuilder = Builders<TEntity>.Filter;
        List<FilterDefinition<TEntity>> filters = new();
        
        foreach (SearchFilterModel searchFilterModel in entitySearch.Search!.Items)
        {
            if (searchFilterModel.Value == null) continue;

            dynamic expression = searchFilterModel.CreateExpression(filterBuilder);
            filters.Add(expression);
        }
        
        Func<List<FilterDefinition<TEntity>>, FilterDefinition<TEntity>> predicate = 
            entitySearch.Search.LinkOperator == Operators.LINK_OPERATOR_OR ? filterBuilder.Or : filterBuilder.And;
            
        return filters.Count > 0 ? predicate(filters) : FilterDefinition<TEntity>.Empty;
    }
    
    public static FilterDefinition<TEntity> BuildFilterDefinition<TEntity>(this List<SearchFilterModel> searchFilters)
    {
        if (searchFilters.Count == 0) 
            return FilterDefinition<TEntity>.Empty;
        
        FilterDefinitionBuilder<TEntity>? filterBuilder = Builders<TEntity>.Filter;
        List<FilterDefinition<TEntity>> filters = new();
        
        foreach (SearchFilterModel searchFilterModel in searchFilters)
        {
            if (searchFilterModel.Value == null) continue;

            dynamic expression = searchFilterModel.CreateExpression(filterBuilder);
            filters.Add(expression);
        }
        
        return filters.Count > 0 ? filterBuilder.And(filters) : FilterDefinition<TEntity>.Empty;
    }
}
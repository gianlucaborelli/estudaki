using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MongoDB.Driver;

namespace Estudaki.Commons.Core.Data.Repository;

public abstract class MongoDbHelper
{
    protected static SortDefinition<T> GetSortDefinition<T>(
    string sortLabel,
    string sortDirection)
    {
        var property = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(x =>
                string.Equals(
                    x.Name,
                    sortLabel,
                    StringComparison.OrdinalIgnoreCase));

        var builder = Builders<T>.Sort;

        if (property is null)
            return builder.Descending("_id");

        var parameter = Expression.Parameter(typeof(T), "x");

        var propertyAccess = Expression.Property(
            parameter,
            property);

        var lambda = Expression.Lambda(
            propertyAccess,
            parameter);

        var field = new ExpressionFieldDefinition<T>(lambda);

        var descending = string.Equals(
            sortDirection,
            "desc",
            StringComparison.OrdinalIgnoreCase);

        return descending
            ? builder.Descending(field)
            : builder.Ascending(field);
    }
}

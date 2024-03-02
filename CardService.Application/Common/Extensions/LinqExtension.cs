using System.Linq.Expressions;

namespace CardService.Application.Common.Extensions
{
    public static class LinqExtension
    {
        public static IEnumerable<TSource> DistinctBy<TSource>(this IEnumerable<TSource> source, Func<TSource, object[]> keySelector)
        {
            var seenKeys = new HashSet<string>();

            foreach (var element in source)
            {
                var keys = keySelector(element);
                var concatenatedKey = string.Join(",", keys.Select(k => k?.ToString() ?? string.Empty));

                if (seenKeys.Add(concatenatedKey))
                {
                    yield return element;
                }
            }
        }

        public static IQueryable<TEntity> OrderByProperty<TEntity>(
        this IQueryable<TEntity> query,
        string propertyName,
        bool ascending = true)
        {
            var entityType = typeof(TEntity);
            var propertyInfo = entityType.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{entityType}'.");
            }

            var parameter = Expression.Parameter(entityType, "x");
            var property = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda(property, parameter);

            var orderByMethod = ascending ? "OrderBy" : "OrderByDescending";
            var orderByExpression = Expression.Call(
                typeof(Queryable),
                orderByMethod,
                new[] { entityType, propertyInfo.PropertyType },
                query.Expression,
                Expression.Quote(lambda)
            );

            return query.Provider.CreateQuery<TEntity>(orderByExpression);
        }

        public static IQueryable<TEntity> ApplyFilter<TEntity>(
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, bool>> filter)
        {
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return query;
        }
    }
}

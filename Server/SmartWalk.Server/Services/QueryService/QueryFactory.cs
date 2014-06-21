using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate;
using SmartWalk.Server.Records;
using SmartWalk.Server.Resources;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.DataContracts.Api;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.QueryService
{
    public static class QueryFactory
    {
        /// <summary>
        /// Generates an expression for a generic query accross table's records with a where filter condition.
        /// </summary>
        public static IQueryable<TRecord> CreateGenericQuery<TRecord>(
            IQueryable<TRecord> table,
            RequestSelect select,
            IDictionary<string, object[]> results)
        {
            var queryable = table;

            // if there is where condition then build where expression
            if (select.Where != null && select.Where.Length > 0)
            {
                var recordExpr = Expression.Parameter(typeof(TRecord), "rec");
                var whereExpr = GetWhereExpression(select.Where, recordExpr, results);
                if (whereExpr != null)
                {
                    var whereCallExpression = Expression.Call(
                        typeof(Queryable),
                        "Where",
                        new[] { typeof(TRecord) },
                        table.Expression,
                        Expression.Lambda<Func<TRecord, bool>>(whereExpr, new[] { recordExpr }));

                    queryable = table.Provider.CreateQuery<TRecord>(whereCallExpression);
                }
            }

            return queryable;
        }

        /// <summary>
        /// Generates an SQL query for a query accross grouped by Host 
        /// EventMetadata table's records with a where condition and order by clause.
        /// </summary>
        public static ISQLQuery CreateGroupedEventsQuery(
            ISession session,
            int limit,
            RequestSelect select,
            IDictionary<string, object[]> results)
        {
            double[] latLong;

            var result =
                session.CreateSQLQuery(
                    string.Format(
                        @"SELECT TOP({6}) {2}.*
                        FROM
	                        (SELECT 
		                        MAX({4}) AS {4}, MAX({3}) AS {3}
	                        FROM 
		                        {0}{1}
	                        GROUP BY
		                        {5}) {2}Groupped
                        INNER JOIN
	                        {0}{1} {2} ON {2}Groupped.{3} = {2}.{3}
                        {7}
                        {8}",
                        QueryContext.Instance.DbPrefix,
                        QueryContext.Instance.EventMetadataTable,
                        QueryContext.Instance.EventMetadataTableAlias,
                        QueryContext.Instance.EventMetadataId,
                        QueryContext.Instance.EventMetadataStartTime,
                        QueryContext.Instance.EventMetadataEntityRecordId,
                        limit,
                        GetWhereString(
                            QueryContext.Instance.EventMetadataTableAlias,
                            select,
                            results),
                        GetSortByString<EventMetadataRecord>(
                            QueryContext.Instance.EventMetadataTableAlias, 
                            select,
                            out latLong)))
                       .AddEntity(typeof(EventMetadataRecord));

            if (latLong != null)
            {
                result = (ISQLQuery)result
                    .SetDouble("lat", latLong[0])
                    .SetDouble("long", latLong[1]);
            }

            if (select.Where != null)
            {
                for (var i = 0; i < select.Where.Length; i++)
                {
                    result = 
                        (ISQLQuery)result
                            .SetParameter("v" + i, select.Where[i].Value);
                }
            }

            return result;
        }

        /// <summary>
        /// Generates an expression for a generic query accross table's records with sorting.
        /// </summary>
        public static IQueryable<TRecord> SortBy<TRecord>(
            this IQueryable<TRecord> table,
            RequestSelect select)
        {
            var queryable = table;

            // if there is sort by condition then build sort by expression
            if (select.SortBy != null && select.SortBy.Length > 0)
            {
                var recordExpr = Expression.Parameter(typeof(TRecord), "rec");
                var expression = queryable.Expression;

                double[] latLong;
                if (typeof(TRecord) == typeof(EventMetadataRecord) && 
                    IsLatLongDistanceSorting(select.SortBy, out latLong))
                {
                    expression = Expression.Call(
                        typeof(Queryable),
                        "OrderBy",
                        new[] { typeof(EventMetadataRecord), typeof(double) },
                        expression,
                        (Expression<Func<EventMetadataRecord, double>>)(emr => 
                            Math.Abs(emr.Latitude - latLong[0]) + 
                            Math.Abs(emr.Longitude - latLong[1])));
                }

                foreach (var sortBy in select.SortBy)
                {
                    if (SkipLatLongDistanceSorting(sortBy)) continue;

                    var sortByExpr = Expression.Property(
                        recordExpr, 
                        recordExpr.Type,
                        GetMappedPropertyName(sortBy.Field, recordExpr.Type));
                    var method =
                        expression == queryable.Expression
                            ? (sortBy.IsDescending.HasValue && sortBy.IsDescending.Value
                                    ? "OrderByDescending"
                                    : "OrderBy")
                            : (sortBy.IsDescending.HasValue && sortBy.IsDescending.Value
                                    ? "ThenByDescending"
                                    : "ThenBy");
                    var methodType = typeof(Func<,>).MakeGenericType(typeof(TRecord), sortByExpr.Type);

                    expression = Expression.Call(
                        typeof(Queryable),
                        method,
                        new[] { typeof(TRecord), sortByExpr.Type },
                        expression,
                        Expression.Lambda(methodType, sortByExpr, new[] { recordExpr }));
                }

                return queryable.Provider.CreateQuery<TRecord>(expression);
            }

            return queryable;
        }

        /// <summary>
        /// Generates a string for a generic query accross table's records with sorting.
        /// </summary>
        private static string GetSortByString<TRecord>(
            string alias,
            RequestSelect select,
            out double[] latLong)
        {
            var result = string.Empty;
            latLong = null;

            // if there is sort by condition then build sort by expression
            if (select.SortBy != null && select.SortBy.Length > 0)
            {
                if (typeof(TRecord) == typeof(EventMetadataRecord) &&
                    IsLatLongDistanceSorting(select.SortBy, out latLong))
                {
                    result = string.Format(
                        "ABS({0}.{1} - :lat) + ABS({0}.{2} - :long) ASC",
                        alias,
                        QueryContext.Instance.EventMetadataLatitude,
                        QueryContext.Instance.EventMetadataLongitude);
                }

                foreach (var sortBy in select.SortBy)
                {
                    if (SkipLatLongDistanceSorting(sortBy)) continue;

                    if (!Reflection<TRecord>.HasProperty(sortBy.Field))
                    {
                        throw new InvalidExpressionException(
                            string.Format(
                                Localization.CantFindFieldInRequestedItems,
                                sortBy.Field));
                    }

                    result +=
                        (result != string.Empty ? ", " : string.Empty) +
                        string.Format(
                            "{0}.{1} {2}",
                            alias,
                            sortBy.Field,
                            sortBy.IsDescending.HasValue &&
                            sortBy.IsDescending.Value
                                ? "DESC"
                                : "ASC");
                }
            }

            return result != string.Empty
                ? " ORDER BY " + result
                : string.Empty;
        }

        private static Expression GetWhereExpression(
            IEnumerable<RequestSelectWhere> whereItems,
            Expression recordExpr,
            IDictionary<string, object[]> results = null)
        {
            var result = default(Expression);

            foreach (var where in whereItems)
            {
                var whereExpr = default(Expression);
                var fields = GetPropertiesPath(where.Field, recordExpr.Type);
                if (fields != null)
                {
                    // building field accessing expression
                    Expression fieldExpr;

                    try
                    {
                        fieldExpr = fields.Aggregate(
                            default(Expression),
                            (current, field) =>
                            Expression.Property(
                                current ?? recordExpr,
                                (current ?? recordExpr).Type,
                                field));
                    }
                    catch (ArgumentException)
                    {
                        throw new InvalidExpressionException(
                            string.Format(
                                Localization.CantFindFieldInRequestedItems,
                                where.Field));
                    }

                    // one value case
                    if (fieldExpr != null &&
                        where.Value != null &&
                        where.Operator == RequestSelectWhereOperators.EqualsTo)
                    {
                        whereExpr = GetWhereValueExpression(fieldExpr, where.Value);
                    }

                    // array of values case
                    if (fieldExpr != null &&
                        where.Values != null &&
                        where.Operator == RequestSelectWhereOperators.EqualsTo)
                    {
                        whereExpr = GetWhereValuesExpression(fieldExpr, where.Values);
                    }

                    // lookup to previous dataset value case
                    if (fieldExpr != null &&
                        where.SelectValue != null &&
                        results != null &&
                        where.Operator == RequestSelectWhereOperators.EqualsTo)
                    {
                        whereExpr = GetWhereSelectValueExpression(fieldExpr, where.SelectValue, results);
                    }
                }

                if (whereExpr != null)
                {
                    result =
                        result == null
                            ? whereExpr
                            : Expression.AndAlso(result, whereExpr);
                }
            }

            return result;
        }

        // TODO: To support the rest of where's value cases
        /// <summary>
        /// Generates a string for a generic query accross table's records with where expression.
        /// </summary>
        private static string GetWhereString(
            string alias, 
            RequestSelect select,
            IDictionary<string, object[]> results)
        {
            var result = string.Empty;

            if (select.Where != null && select.Where.Length > 0)
            {
                for (var i = 0; i < select.Where.Length; i++)
                {
                    var where = select.Where[i];
                    if (QueryContext.Instance.EventMetadataProperties
                            .Contains(where.Field, StringComparer.OrdinalIgnoreCase))
                    {
                        if (RequestSelectWhereOperators.All
                                .Contains(where.Operator, StringComparer.OrdinalIgnoreCase))
                        {
                            result +=
                                (result != string.Empty ? " AND " : string.Empty) +
                                string.Format(
                                    "({0}.{1}{2}:v{3})",
                                    alias,
                                    where.Field,
                                    where.Operator,
                                    i);
                        }
                        else
                        {
                            throw new InvalidExpressionException(
                                string.Format("Operator '{0}' is not supported.", where.Operator));
                        }
                    }
                    else
                    {
                        throw new InvalidExpressionException(
                            string.Format("Can't find field '{0}'.", where.Field));
                    }
                }
            }

            return result != string.Empty
                ? " WHERE " + result
                : string.Empty;
        }

        private static Expression GetWhereValueExpression(Expression fieldExpr, object value)
        {
            var valueExpr = Expression.Convert(Expression.Constant(value), fieldExpr.Type);
            var result = Expression.Equal(fieldExpr, valueExpr);
            return result;
        }

        private static Expression GetWhereValuesExpression(Expression fieldExpr, IEnumerable<object> values)
        {
            var valuesExpr = Expression.Constant(values.ToArray());
            var result = Expression.Call(
                typeof(Enumerable),
                "Contains",
                new[] { typeof(object) },
                valuesExpr,
                Expression.Convert(fieldExpr, typeof(object)));
            return result;
        }

        private static Expression GetWhereSelectValueExpression(
            Expression fieldExpr,
            RequestSelectWhereSelectValue selectValue,
            IDictionary<string, object[]> results)
        {
            Expression result;

            object[] lookUpRecords;
            if (!results.TryGetValue(selectValue.SelectName, out lookUpRecords) ||
                lookUpRecords == null)
            {
                throw new InvalidExpressionException(
                    string.Format(
                        Localization.CantAccessSelectResultItems,
                        selectValue.SelectName));
            }

            if (lookUpRecords.Length > 0)
            {
                // resolving the type of records in look up dataset
                var recordType = lookUpRecords.First().GetType();
                var fields = GetPropertiesPath(selectValue.Field);

                // filling up the cache of property path reflection info
                var propertyInfos = GetPropertyInfoCache(recordType, fields);
                if (propertyInfos == null)
                {
                    throw new InvalidExpressionException(
                        string.Format(
                            Localization.CantFineFieldInSelectResultItems,
                            selectValue.Field,
                            selectValue.SelectName));
                }

                // extracting the values of requested fields path from lookup dataset
                var lookUpValues = lookUpRecords
                    .SelectMany(v => GetLookUpValues(v, propertyInfos))
                    .Where(v => v != null)
                    .Distinct()
                    .ToArray();

                result = GetWhereValuesExpression(fieldExpr, lookUpValues);
            }
            else
            {
                result = Expression.Constant(false);
            }

            return result;
        }

        /// <summary>
        /// Recursivelly goes throw the items according to property path and returns the union of values. 
        /// Supports simple and enumerable nested types.
        /// </summary>
        private static IEnumerable<object> GetLookUpValues(object item, IEnumerable<PropertyInfo> propertyInfos)
        {
            // skipping all references to externa storages
            var reference = item as IReference;
            if (reference != null && reference.Storage != StorageKeys.SmartWalk) return Enumerable.Empty<object>();

            var result = new List<object>();
            var pathStack = new Stack<PropertyInfo>(propertyInfos.Reverse());
            var lastValue = item;

            while (pathStack.Count > 0)
            {
                var propertyInfo = pathStack.Pop();
                if (propertyInfo.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
                {
                    var value = propertyInfo.GetValue(lastValue, null);
                    var enumerableValue = (IEnumerable<object>)value;
                    lastValue = enumerableValue
                        .SelectMany(v => GetLookUpValues(v, pathStack.ToArray()));
                    break;
                }

                lastValue = propertyInfo.GetValue(lastValue, null);
            }

            var enumerableLastValue = lastValue as IEnumerable<object>;
            if (enumerableLastValue != null)
            {
                result.AddRange(enumerableLastValue);
            }
            else
            {
                result.Add(lastValue);
            }

            return result.ToArray();
        }

        // TODO: To cache in app memory precalculated properties to avoid reflection for every http request
        /// <summary>
        /// Gets the pre-calculated set of reflection property infos 
        /// that is used for better performance while accessing lookup values.
        /// </summary>
        private static IEnumerable<PropertyInfo> GetPropertyInfoCache(Type recordType, IEnumerable<string> fields)
        {
            var result = new List<PropertyInfo>();
            var lastType = recordType;

            foreach (var field in fields)
            {
                var propertyInfo = lastType.GetProperty(
                    field,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null) return null;

                result.Add(propertyInfo);

                var elementType = ReflectionExtensions.GetElementType(propertyInfo.PropertyType);
                lastType = elementType ?? propertyInfo.PropertyType;
            }

            return result;
        }

        /// <summary>
        /// Returns the list of properties extracted from raw fieldsPath string. 
        /// </summary>
        /// <returns>The list of properties. All properties except the last one are updated with postfix 'Record' 
        /// (if pathToRecordMode equals true) assuming that all complex properies are records.</returns>
        private static IEnumerable<string> GetPropertiesPath(
            string fieldsPath, 
            Type targetRecordType = null)
        {
            if (fieldsPath == null) return null;

            string[] result;

            if (fieldsPath.Contains("."))
            {
                var properties = fieldsPath.Split('.');
                result =
                    targetRecordType != null
                        ? properties
                              .Select(p => GetMappedPropertyName(p, targetRecordType))
                              .ToArray()
                        : properties;
            }
            else
            {
                result = new[] { fieldsPath };
            }

            return result;
        }

        private static string GetMappedPropertyName(
            string property, 
            Type targetRecordType)
        {
            if (targetRecordType == typeof(EventMetadataRecord))
            {
                if (property == QueryContext.Instance.EventMetadataHost)
                {
                    return QueryContext.Instance.EventMetadataEntityRecord;
                }
            }

            if (targetRecordType == typeof(ShowRecord))
            {
                if (property == QueryContext.Instance.ShowVenue)
                {
                    return QueryContext.Instance.ShowEntityRecord;
                }
            }

            return property;
        }

        private static bool IsLatLongDistanceSorting(
            RequestSelectSortBy[] sortBy,
            out double[] latLong)
        {
            var latitude = sortBy
                .FirstOrDefault(
                    sb => sb.Field.EqualsIgnoreCase(QueryContext.Instance.EventMetadataLatitude));
            var longitude = sortBy
                .FirstOrDefault(
                    sb => sb.Field.EqualsIgnoreCase(QueryContext.Instance.EventMetadataLongitude));

            if (latitude != null && latitude.OfDistance.HasValue &&
                longitude != null && longitude.OfDistance.HasValue)
            {
                latLong = new[] { latitude.OfDistance.Value, longitude.OfDistance.Value };
                return true;
            }

            latLong = null;
            return false;
        }

        private static bool SkipLatLongDistanceSorting(
            RequestSelectSortBy sortBy)
        {
            return (sortBy.Field == QueryContext.Instance.EventMetadataLatitude ||
                    sortBy.Field == QueryContext.Instance.EventMetadataLongitude) && 
                    sortBy.OfDistance.HasValue;
        }
    }
}
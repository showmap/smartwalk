using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Server.Services
{
    [UsedImplicitly]
    public class QueryService : IQueryService
    {
        private const int DefaultLimit = 100;
        private const string RecordPostfix = "Record";

        private readonly IRepository<EventMetadataRecord> _eventMetadataRepository;
        private readonly IRepository<EntityRecord> _entityRepository;
        private readonly IRepository<ShowRecord> _showRepository;

        public QueryService(IRepository<EventMetadataRecord> eventMetadataRepository,
            IRepository<EntityRecord> entityRepository,
            IRepository<ShowRecord> showRepository)
        {
            _eventMetadataRepository = eventMetadataRepository;
            _entityRepository = entityRepository;
            _showRepository = showRepository;
        }

        public Response ExecuteQuery(Request request)
        {
            if (request == null || request.Selects == null) return new Response();

            var resultSelects = new Dictionary<string, object[]>();
            var i = 0;

            foreach (var select in request.Selects)
            {
                var records = ExecuteSelect(select, resultSelects);
                resultSelects.Add(select.As ?? string.Format("result_{0}", i++), records);
            }

            return new Response
                {
                    Selects = resultSelects
                        .Select(kvp => new ResponseSelect {Alias = kvp.Key, Records = kvp.Value})
                        .ToArray()
                };
        }

        // ReSharper disable CoVariantArrayConversion
        private object[] ExecuteSelect(
            RequestSelect select, 
            IDictionary<string, object[]> results)
        {
            if (select.From == null) return null;

            switch (select.From)
            {
                case RequestSelectFromTables.EventMetadata:
                    return ExecuteSelect(_eventMetadataRepository.Table, select, results);

                case RequestSelectFromTables.Entity:
                    return ExecuteSelect(_entityRepository.Table, select, results);

                case RequestSelectFromTables.Show:
                    return ExecuteSelect(_showRepository.Table, select, results);
            }

            return null;
        }
        // ReSharper restore CoVariantArrayConversion

        private TRecord[] ExecuteSelect<TRecord>(
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
                        new[] {typeof(TRecord)},
                        table.Expression,
                        Expression.Lambda<Func<TRecord, bool>>(whereExpr, new[] {recordExpr}));

                    queryable = _eventMetadataRepository.Table.Provider
                        .CreateQuery<TRecord>(whereCallExpression);
                }
            }

            var result = queryable.Take(DefaultLimit).ToArray();
            return result;
        }

        private static Expression GetWhereExpression(
            IEnumerable<RequestSelectWhere> whereItems, 
            Expression recordExpr,
            IDictionary<string, object[]> results = null)
        {
            var resultExpr = default(Expression);

            foreach (var where in whereItems)
            {
                var whereExpr = default(Expression);
                var fields = GetWhereFields(where.Field);
                if (fields != null)
                {
                    // building field accessing expression
                    var fieldExpr = fields.Aggregate(
                        default(Expression), 
                        (current, field) => 
                            Expression.Property(
                                current ?? recordExpr, 
                                (current ?? recordExpr).Type, 
                                field));

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
                        results.ContainsKey(where.SelectValue.SelectName) &&
                        where.Operator == RequestSelectWhereOperators.EqualsTo)
                    {
                        whereExpr = GetWhereSelectValueExpression(fieldExpr, where.SelectValue, results);
                    }
                }

                if (whereExpr != null)
                {
                    resultExpr = resultExpr == null ? whereExpr : Expression.AndAlso(resultExpr, whereExpr);
                }
            }

            return resultExpr;
        }

        private static Expression GetWhereValueExpression(Expression fieldExpr, string value)
        {
            var valueExpr = Expression.Constant(value);
            var whereExpr = Expression.Equal(fieldExpr, valueExpr);
            return whereExpr;
        }

        private static Expression GetWhereValuesExpression(Expression fieldExpr, IEnumerable<object> values)
        {
            var valuesExpr = Expression.Constant(values.ToArray());
            var whereExpr = Expression.Call(
                typeof(Enumerable), 
                "Contains",
                new[] { typeof(object) }, 
                valuesExpr,
                Expression.Convert(fieldExpr, typeof(object)));
            return whereExpr;
        }

        private static Expression GetWhereSelectValueExpression(
            Expression fieldExpr,
            RequestSelectWhereSelectValue selectValue,
            IDictionary<string, object[]> results)
        {
            var whereExpr = default(Expression);
            var lookUpRecords = results[selectValue.SelectName];
            if (lookUpRecords != null && lookUpRecords.Length > 0)
            {
                var fields = GetWhereFields(selectValue.Field);

                // resolving the type of records in look up dataset
                var lastType = lookUpRecords.First().GetType();

                // filling up the cache of property path reflection info
                var propInfos = new List<PropertyInfo>();
                foreach (var field in fields)
                {
                    var propertyInfo = lastType.GetProperty(field);
                    propInfos.Add(propertyInfo);
                    lastType = propertyInfo.PropertyType;
                }
                
                // extracting the values of requested fields path from look up dataset
                var lookUpValues = lookUpRecords
                    .Select(rec => propInfos.Aggregate(rec, (cur, pi) => pi.GetValue(cur, null)))
                    .Where(v => v != null)
                    .Distinct()
                    .ToArray();

                whereExpr = GetWhereValuesExpression(fieldExpr, lookUpValues);
            }

            return whereExpr;
        }

        /// <summary>
        /// Returns the list of fields extracted from raw field string. 
        /// All fields except the last one are updated with postfix 'Record' assuming that all complex properies are records.
        /// </summary>
        private static IEnumerable<string> GetWhereFields(string field)
        {
            if (field == null) return null;

            string[] fields;

            if (field.Contains("."))
            {
                fields = field.Split('.');
                fields = fields
                    .Select((f, i) => i < fields.Length - 1 ? f + RecordPostfix : f)
                    .ToArray();
            }
            else
            {
                fields = new[] { field };
            }

            return fields;
        }
    }
}
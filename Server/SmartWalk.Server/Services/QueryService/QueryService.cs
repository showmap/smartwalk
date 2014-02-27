using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Server.Services.QueryService
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
            var resultSelectDataContracts = new Dictionary<string, object[]>();
            var errors = new Dictionary<string, string>();
            var i = 0;

            foreach (var select in request.Selects)
            {
                var alias = select.As ?? string.Format("result_{0}", i++);

                try
                {
                    var result = ExecuteSelect(select, resultSelects);

                    resultSelects.Add(alias, result.Records);
                    resultSelectDataContracts.Add(alias, result.DataContracts);
                }
                catch (InvalidExpressionException ex)
                {
                    resultSelectDataContracts.Add(alias, null);
                    errors.Add(alias, ex.Message);
                }
            }

            return new Response
                {
                    Selects = resultSelectDataContracts
                        .Select(kvp =>
                                new ResponseSelect
                                    {
                                        Alias = kvp.Key,
                                        Records = kvp.Value,
                                        Error = errors.ContainsKey(kvp.Key) ? errors[kvp.Key] : null
                                    })
                        .ToArray()
                };
        }

        // ReSharper disable CoVariantArrayConversion
        private ExecuteSelectResult ExecuteSelect(
            RequestSelect select, 
            IDictionary<string, object[]> results)
        {
            if (select.From == null) return null;

            switch (select.From)
            {
                case RequestSelectFromTables.EventMetadata:
                    var emRecords = ExecuteSelect(_eventMetadataRepository.Table, select, results);
                    var emDataContr = emRecords
                        .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields))
                        .ToArray();
                    return new ExecuteSelectResult(emRecords, emDataContr);

                case RequestSelectFromTables.Entity:
                    var entityRecords = ExecuteSelect(_entityRepository.Table, select, results);
                    var entityDataContr = entityRecords
                        .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields))
                        .ToArray();
                    return new ExecuteSelectResult(entityRecords, entityDataContr);

                case RequestSelectFromTables.Show:
                    var showRecords = ExecuteSelect(_showRepository.Table, select, results);
                    var showDataContr = showRecords
                        .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields))
                        .ToArray();
                    return new ExecuteSelectResult(showRecords, showDataContr);
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
            var result = default(Expression);

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
                        where.Operator == RequestSelectWhereOperators.EqualsTo)
                    {
                        whereExpr = GetWhereSelectValueExpression(fieldExpr, where.SelectValue, results);
                    }
                }

                if (whereExpr != null)
                {
                    result = result == null ? whereExpr : Expression.AndAlso(result, whereExpr);
                }
            }

            return result;
        }

        private static Expression GetWhereValueExpression(Expression fieldExpr, object value)
        {
            var valueExpr = Expression.Constant(value);
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
            var result = default(Expression);
            
            if (!results.ContainsKey(selectValue.SelectName))
            {
                throw new InvalidExpressionException(
                    string.Format(
                    "Can not access '{0}' select result items.",
                    selectValue.SelectName));
            }

            var lookUpRecords = results[selectValue.SelectName];
            if (lookUpRecords != null && lookUpRecords.Length > 0)
            {
                var fields = GetWhereFields(selectValue.Field);

                // resolving the type of records in look up dataset
                var recordType = lookUpRecords.First().GetType();

                // filling up the cache of property path reflection info
                var propInfos = GetPropertyPath(recordType, fields);
                if (propInfos == null)
                {
                    throw new InvalidExpressionException(
                        string.Format(
                        "Can not find '{0}' property in '{1}' select result items.",
                        selectValue.Field,
                        selectValue.SelectName)); 
                }
                
                // extracting the values of requested fields path from look up dataset
                var lookUpValues = lookUpRecords
                    .Select(rec => propInfos.Aggregate(rec, (cur, pi) => pi.GetValue(cur, null)))
                    .Where(v => v != null)
                    .Distinct()
                    .ToArray();

                result = GetWhereValuesExpression(fieldExpr, lookUpValues);
            }

            return result;
        }

        private static IEnumerable<PropertyInfo> GetPropertyPath(Type recordType, IEnumerable<string> fields)
        {
            var result = new List<PropertyInfo>();
            var lastType = recordType;

            foreach (var field in fields)
            {
                var propertyInfo = lastType.GetProperty(field);
                if (propertyInfo == null) return null;

                result.Add(propertyInfo);
                lastType = propertyInfo.PropertyType;
            }

            return result;
        }

        /// <summary>
        /// Returns the list of fields extracted from raw field string. 
        /// All fields except the last one are updated with postfix 'Record' assuming that all complex properies are records.
        /// </summary>
        private static IEnumerable<string> GetWhereFields(string field)
        {
            if (field == null) return null;

            string[] result;

            if (field.Contains("."))
            {
                var fields = field.Split('.');
                result = fields
                    .Select((f, i) => i < fields.Length - 1 ? f + RecordPostfix : f)
                    .ToArray();
            }
            else
            {
                result = new[] { field };
            }

            return result;
        }

        private class ExecuteSelectResult
        {
            public ExecuteSelectResult(object[] records, object[] dataContracts)
            {
                Records = records;
                DataContracts = dataContracts;
            }

            public object[] Records { get; private set; }
            public object[] DataContracts { get; private set; }
        }
    }
}
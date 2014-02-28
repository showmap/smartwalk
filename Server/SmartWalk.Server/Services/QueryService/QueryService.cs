using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Shared.DataContracts.Api;
using SmartWalk.Shared.Extensions;
using SmartWalk.Server.Extensions;

namespace SmartWalk.Server.Services.QueryService
{
    [UsedImplicitly]
    public class QueryService : IQueryService
    {
        private const int DefaultLimit = 100;

        private const string WhereMethod = "Where";
        private const string ContainsMethod = "Contains";
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
            var errors = new Dictionary<string, string>();
            var storages = 
                request.Storages != null
                    ? request.Storages
                        .Intersect(StorageKeys.All, StringComparer.OrdinalIgnoreCase)
                        .ToArray()
                    : null;
            var i = 0;

            foreach (var select in request.Selects)
            {
                var alias = select.As ?? string.Format("result_{0}", i++);

                try
                {
                    var dataContracts = ExecuteSelect(select, storages, resultSelects);
                    resultSelects.Add(alias, dataContracts);
                }
                catch (Exception ex)
                {
                    if (ex is InvalidExpressionException || ex is ApplicationException)
                    {
                        resultSelects.Add(alias, null);

                        errors.Add(
                            alias,
                            ex.InnerException != null
                                ? ex.InnerException.Message
                                : ex.Message);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return new Response
                {
                    Selects = resultSelects
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

        private object[] ExecuteSelect(
            RequestSelect select,
            string[] storages,
            IDictionary<string, object[]> results)
        {
            if (select.From == null) throw new InvalidExpressionException("Select.From value can not be null.");

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.EventMetadata))
            {
                var records = ExecuteSelect(_eventMetadataRepository.Table, select, results);
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields, storages))
                    .ToArray();
                return dataContracts;
            }

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.Entity))
            {
                var records = ExecuteSelect(_entityRepository.Table, select, results);
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields))
                    .ToArray();
                return dataContracts;
            }

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.Show))
            {
                var records = ExecuteSelect(_showRepository.Table, select, results);
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields, storages))
                    .ToArray();
                return dataContracts;
            }

            throw new InvalidExpressionException(string.Format("Can not find '{0}' table", select.From));
        }

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
                        WhereMethod,
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
                var fields = GetFieldsPath(where.Field, true);
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
                ContainsMethod,
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

            object[] lookUpRecords;
            if (!results.TryGetValue(selectValue.SelectName, out lookUpRecords) || 
                lookUpRecords == null)
            {
                throw new InvalidExpressionException(
                    string.Format(
                    "Can not access '{0}' select result items.",
                    selectValue.SelectName));
            }

            if (lookUpRecords.Length > 0)
            {
                var fields = GetFieldsPath(selectValue.Field);

                // resolving the type of records in look up dataset
                var recordType = lookUpRecords.First().GetType();

                // filling up the cache of property path reflection info
                var propertyInfos = GetPropertyInfoCache(recordType, fields);
                if (propertyInfos == null)
                {
                    throw new InvalidExpressionException(
                        string.Format(
                        "Can not find '{0}' property in '{1}' select result items.",
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

            return result;
        }

        /// <summary>
        /// Recursivelly goes throw the items according to property path and returns the union of values. 
        /// Supports simple and enumerable nested types.
        /// </summary>
        private static IEnumerable<object> GetLookUpValues(object item, IEnumerable<PropertyInfo> propertyInfos)
        {
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
        /// Returns the list of fields extracted from raw field string. 
        /// </summary>
        /// <returns>The list of fields. All fields except the last one are updated with postfix 'Record' 
        /// (if pathToRecordMode equals true) assuming that all complex properies are records.</returns>
        private static IEnumerable<string> GetFieldsPath(string field, bool pathToRecordMode = false)
        {
            if (field == null) return null;

            string[] result;

            if (field.Contains("."))
            {
                var fields = field.Split('.');
                result =
                    pathToRecordMode
                        ? fields
                              .Select((f, i) => i < fields.Length - 1 ? f + RecordPostfix : f)
                              .ToArray()
                        : fields;
            }
            else
            {
                result = new[] {field};
            }

            return result;
        }
    }
}
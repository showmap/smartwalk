using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Server.Services
{
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

            var resultSelects = new Dictionary<string, Tuple<Type, object[]>>();
            var i = 0;

            foreach (var select in request.Selects)
            {
                var tuple = ExecuteSelect(select, resultSelects);
                resultSelects.Add(select.As ?? string.Format("result_{0}", i++), tuple);
            }

            return new Response
                {
                    Selects = resultSelects
                        .Select(kvp => new ResponseSelect {Alias = kvp.Key, Records = kvp.Value.Item2})
                        .ToArray()
                };
        }

        // ReSharper disable CoVariantArrayConversion
        private Tuple<Type, object[]> ExecuteSelect(
            RequestSelect select, 
            IDictionary<string, Tuple<Type, object[]>> results)
        {
            if (select.From == null) return null;

            switch (select.From)
            {
                case RequestSelectFromTables.EventMetadata:
                    return new Tuple<Type, object[]>(
                        typeof(EventMetadataRecord),
                        ExecuteSelect(_eventMetadataRepository.Table, select, results));

                case RequestSelectFromTables.Entity:
                    return new Tuple<Type, object[]>(
                        typeof(EntityRecord),
                        ExecuteSelect(_entityRepository.Table, select, results));

                case RequestSelectFromTables.Show:
                    return new Tuple<Type, object[]>(
                        typeof(ShowRecord),
                        ExecuteSelect(_showRepository.Table, select, results));
            }

            return null;
        }
        // ReSharper restore CoVariantArrayConversion

        private TRecord[] ExecuteSelect<TRecord>(
            IQueryable<TRecord> table,
            RequestSelect select,
            IDictionary<string, Tuple<Type, object[]>> results)
        {
            IQueryable<TRecord> queryable;

            if (select.Where != null && select.Where.Length > 0)
            {
                var paramExpr = Expression.Parameter(typeof(TRecord), "rec");
                var whereExpr = GetWhereExpression<TRecord>(select.Where, paramExpr, results);
                if (whereExpr != null)
                {
                    var whereCallExpression = Expression.Call(
                        typeof(Queryable),
                        "Where",
                        new[] {typeof(TRecord)},
                        table.Expression,
                        Expression.Lambda<Func<TRecord, bool>>(whereExpr, new[] {paramExpr}));

                    queryable = _eventMetadataRepository.Table.Provider
                        .CreateQuery<TRecord>(whereCallExpression);
                }
                else
                {
                    queryable = table;
                }
            }
            else
            {
                queryable = table;
            }

            var result = queryable.Take(DefaultLimit).ToArray();
            return result;
        }

        private Expression GetWhereExpression<TRecord>(
            IEnumerable<RequestSelectWhere> whereItems, 
            ParameterExpression paramExpr,
            IDictionary<string, Tuple<Type, object[]>> results = null)
        {
            return GetWhereExpression(typeof(TRecord), whereItems, paramExpr, results);
        }

        private Expression GetWhereExpression(
            Type type,
            IEnumerable<RequestSelectWhere> whereItems,
            ParameterExpression paramExpr,
            IDictionary<string, Tuple<Type, object[]>> results = null)
        {
            var resultExpr = default(Expression);

            foreach (var where in whereItems)
            {
                var whereExpr = default(Expression);
                var fields = GetWhereFields(where);
                if (fields != null)
                {
                    var fieldExpr = default(Expression);
                    var lastType = type;

                    foreach (var field in fields)
                    {   
                        fieldExpr = Expression.Property(fieldExpr ?? paramExpr, lastType, field);
                        lastType = lastType.GetProperty(field).PropertyType;
                    }

                    if (fieldExpr != null &&
                        where.Value != null && 
                        where.Operator == RequestSelectWhereOperators.EqualsTo)
                    {
                        var valueExpr = Expression.Constant(where.Value, typeof(string));
                        whereExpr = Expression.Equal(fieldExpr, valueExpr);
                    }

                    if (fieldExpr != null &&
                        where.Values != null &&
                        where.Operator == RequestSelectWhereOperators.EqualsTo)
                    {
                        // TODO:
                    }

                    if (fieldExpr != null &&
                        where.SelectValue != null &&
                        results != null &&
                        results.ContainsKey(where.SelectValue.SelectName) &&
                        where.Operator == RequestSelectWhereOperators.EqualsTo)
                    {
                        // TODO:
                    }
                }

                if (whereExpr != null)
                {
                    resultExpr = resultExpr == null ? whereExpr : Expression.AndAlso(resultExpr, whereExpr);
                }
            }

            return resultExpr;
        }

        private string[] GetWhereFields(RequestSelectWhere where)
        {
            if (where.Field == null) return null;

            string[] fields;

            if (where.Field.Contains("."))
            {
                fields = where.Field.Split('.');
                fields = fields.Select((f, i) => i < fields.Length - 1 ? f + RecordPostfix : f).ToArray();
            }
            else
            {
                fields = new[] {where.Field};
            }

            return fields;
        }
    }
}
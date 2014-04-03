using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using JetBrains.Annotations;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Shared.DataContracts.Api;
using SmartWalk.Server.Extensions;
using Orchard.Environment.Configuration;
using NHibernate;

namespace SmartWalk.Server.Services.QueryService
{
    [UsedImplicitly]
    public class QueryService : IQueryService
    {
        private const int DefaultLimit = 100;

        private readonly IRepository<EventMetadataRecord> _eventMetadataRepository;
        private readonly IRepository<EntityRecord> _entityRepository;
        private readonly IRepository<ShowRecord> _showRepository;

        private readonly ISessionLocator _sessionLocator;
        private readonly ShellSettings _shellSettings;

        public QueryService(
            IRepository<EventMetadataRecord> eventMetadataRepository,
            ISessionLocator sessionLocator, ShellSettings shellSettings,
            IRepository<EntityRecord> entityRepository,
            IRepository<ShowRecord> showRepository)
        {
            _eventMetadataRepository = eventMetadataRepository;
            _entityRepository = entityRepository;
            _showRepository = showRepository;

            _sessionLocator = sessionLocator;
            _shellSettings = shellSettings;
        }

        #region Execute raw sql example
        public void Test()
        {
            var session = _sessionLocator.For(typeof(EventMetadataRecord));

            var query = session.CreateSQLQuery(string.Format(@"
                        SELECT Id as myId, Description, StartTime
                        FROM {0}SmartWalk_Server_EventMetadataRecord
                        WHERE EntityRecord_Id = :entityid AND DTime >= :dtfrom
                    ", !string.IsNullOrEmpty(_shellSettings.DataTablePrefix) ? _shellSettings.DataTablePrefix + "_" : ""))
                           .AddScalar("myId", NHibernateUtil.Int32)
                           .AddScalar("Description", NHibernateUtil.String)
                           .AddScalar("StartTime", NHibernateUtil.DateTime)
                           .SetParameter("entityid", 1)
                           .SetParameter("dtfrom", DateTime.Now.AddMonths(-3));
            var result = query.List<object[]>().Select(r => new TestResult
            {
                Id = (int)r[0],
                Description = (string)r[1],
                MyDate = (DateTime)r[23]
            }); //.UniqueResult<Int32>(); If query returns one value
        }

        private class TestResult
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public DateTime MyDate { get; set; }
        }
        #endregion   

        public Response ExecuteRequestQuery(Request request)
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
                    var dataContracts = ExecuteSelectQuery(select, storages, resultSelects);
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

        private object[] ExecuteSelectQuery(
            RequestSelect select,
            string[] storages,
            IDictionary<string, object[]> results)
        {
            if (select.From == null) throw new InvalidExpressionException("Select.From value can not be null.");

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.EventMetadata))
            {
                var queryable = ExpressionFactory.CreateGenericQuery(_eventMetadataRepository.Table, select, results);
                var records = queryable.SortBy(select).Take(DefaultLimit).ToArray();
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields, storages))
                    .ToArray();
                return dataContracts;
            }

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.GroupedEventMetadata))
            {
                var queryable = ExpressionFactory.CreateGroupedEventsQuery(_entityRepository.Table, select, results);
                var records = queryable.SortBy(select).Take(DefaultLimit).ToArray();
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields, storages))
                    .ToArray();
                return dataContracts;
            }

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.Entity))
            {
                var queryable = ExpressionFactory.CreateGenericQuery(_entityRepository.Table, select, results);
                var records = queryable.SortBy(select).Take(DefaultLimit).ToArray();
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields))
                    .ToArray();
                return dataContracts;
            }

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.Show))
            {
                var queryable = ExpressionFactory.CreateGenericQuery(_showRepository.Table, select, results);
                var records = queryable.SortBy(select).Take(DefaultLimit).ToArray();
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields, storages))
                    .ToArray();
                return dataContracts;
            }

            throw new InvalidExpressionException(string.Format("Can not find '{0}' table", select.From));
        }
    }
}
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
                var queryable = QueryFactory.CreateGenericQuery(_eventMetadataRepository.Table, select, results);
                var records = queryable.SortBy(select).Take(DefaultLimit).ToArray();
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields, storages))
                    .ToArray();
                return dataContracts;
            }

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.GroupedEventMetadata))
            {
                var queryable = QueryFactory.CreateGroupedEventsQuery(
                    _sessionLocator.For(typeof(EventMetadataRecord)),
                    _shellSettings,
                    select,
                    results);
                var records = queryable.List<EventMetadataRecord>();
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields, storages))
                    .ToArray();
                return dataContracts;
            }

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.Entity))
            {
                var queryable = QueryFactory.CreateGenericQuery(_entityRepository.Table, select, results);
                var records = queryable.SortBy(select).Take(DefaultLimit).ToArray();
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields))
                    .ToArray();
                return dataContracts;
            }

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.Show))
            {
                var queryable = QueryFactory.CreateGenericQuery(_showRepository.Table, select, results);
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
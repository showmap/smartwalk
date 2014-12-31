using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Orchard.Data;
using Orchard.Environment.Configuration;
using Orchard.FileSystems.Media;
using Orchard.MediaProcessing.Services;
using SmartWalk.Server.Records;
using SmartWalk.Server.Resources;
using SmartWalk.Shared;
using SmartWalk.Shared.DataContracts.Api;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.QueryService
{
    [UsedImplicitly]
    public class QueryService : IQueryService
    {
        public const int DefaultEventsLimit = 30;
        private const int DefaultEntitiesLimit = 200;
        private const int DefaultShowsLimit = 1000;

        private readonly IRepository<EventMetadataRecord> _eventMetadataRepository;
        private readonly IRepository<EntityRecord> _entityRepository;
        private readonly IRepository<EventEntityDetailRecord> _eventEntityDetailRepository;
        private readonly IRepository<ShowRecord> _showRepository;

        private readonly ISessionLocator _sessionLocator;
        private readonly IStorageProvider _storageProvider;
        private readonly IImageProfileManager _imageProfileManager;

        public QueryService(
            IRepository<EventMetadataRecord> eventMetadataRepository,
            ISessionLocator sessionLocator,
            ShellSettings shellSettings,
            IRepository<EntityRecord> entityRepository,
            IRepository<EventEntityDetailRecord> eventEntityDetailRepository,
            IRepository<ShowRecord> showRepository,
            IStorageProvider storageProvider,
            IImageProfileManager imageProfileManager)
        {
            _eventMetadataRepository = eventMetadataRepository;
            _entityRepository = entityRepository;
            _eventEntityDetailRepository = eventEntityDetailRepository;
            _showRepository = showRepository;
            _storageProvider = storageProvider;
            _imageProfileManager = imageProfileManager;

            _sessionLocator = sessionLocator;

            QueryContext.InstantiateContext(shellSettings);
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

        // ReSharper disable CoVariantArrayConversion
        private object[] ExecuteSelectQuery(
            RequestSelect select,
            string[] storages,
            IDictionary<string, object[]> results)
        {
            if (select.From == null) throw new InvalidExpressionException(Localization.SelectFromCantBeNull);

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.EventMetadata))
            {
                var queryable = GenericQueryFactory.CreateQuery(_eventMetadataRepository.Table, select, results);
                var records = queryable
                    .Where(r => !r.IsDeleted && r.Status == (byte)EventStatus.Public)
                    .OrderBy(select)
                    .Take(DefaultEventsLimit)
                    .ToArray();
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields, storages,
                        _storageProvider, select.PictureSize, _imageProfileManager))
                    .ToArray();

                return dataContracts;
            }

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.GroupedEventMetadata))
            {
                var session = _sessionLocator.For(typeof(EventMetadataRecord));
                var sqlQuery = GroupedEventsQueryFactory.CreateQuery(
                    session,
                    select.AppendDefaultWhere(),
                    results);
                var records = sqlQuery.List<EventMetadataRecord>();
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields, storages,
                        _storageProvider, select.PictureSize, _imageProfileManager))
                    .ToArray();
                return dataContracts;
            }

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.Entity))
            {
                var queryable = GenericQueryFactory.CreateQuery(_entityRepository.Table, select, results);
                var records = queryable
                    .Where(r => !r.IsDeleted)
                    .OrderBy(select)
                    .Take(DefaultEntitiesLimit)
                    .ToArray();
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields,
                        _storageProvider, select.PictureSize, _imageProfileManager))
                    .ToArray();
                return dataContracts;
            }

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.EventVenueDetail))
            {
                var queryable = GenericQueryFactory.CreateQuery(_eventEntityDetailRepository.Table, select, results);
                var records = queryable
                    .Where(r => !r.IsDeleted)
                    .OrderBy(select)
                    .Take(DefaultEntitiesLimit)
                    .ToArray();
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields, storages))
                    .ToArray();
                return dataContracts;
            }

            if (select.From.EqualsIgnoreCase(RequestSelectFromTables.Show))
            {
                var queryable = GenericQueryFactory.CreateQuery(_showRepository.Table, select, results);
                var records = queryable
                    .Where(r => !r.IsDeleted)
                    .OrderBy(select)
                    .Take(DefaultShowsLimit)
                    .ToArray();
                var dataContracts = records
                    .Select(rec => DataContractsFactory.CreateDataContract(rec, select.Fields, storages,
                        _storageProvider, select.PictureSize, _imageProfileManager))
                    .ToArray();
                return dataContracts;
            }

            throw new InvalidExpressionException(string.Format(Localization.CantFindTable, select.From));
        }
        // ReSharper restore CoVariantArrayConversion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Shared.DataContracts.Api;
using SmartWalk.Server.Extension;

namespace SmartWalk.Server.Services
{
    public class QueryService : IQueryService {
        private readonly IRepository<EventMetadataRecord> _eventMetadataRepository;
        private readonly IRepository<RegionRecord> _regionRepository;
        private readonly IRepository<EntityRecord> _entityRepository;
        private readonly IRepository<ShowRecord> _showRepository;

        public QueryService(IRepository<EventMetadataRecord> eventMetadataRepository, IRepository<RegionRecord> regionRepository,
                            IRepository<EntityRecord> entityRepository, IRepository<ShowRecord> showRepository) {
            _eventMetadataRepository = eventMetadataRepository;
            _regionRepository = regionRepository;
            _entityRepository = entityRepository;
            _showRepository = showRepository;
        }

        public Response ExecuteQuery(Request request) {
            
            IDictionary<string, object[]> res = new Dictionary<string, object[]>();
            var i = 0;
            foreach (var select in request.Selects) {
                res.Add(select.As ?? string.Format("result_{0}", i++), ExecuteSelect(select, res));
            }

            return new Response {Results = res.Values};
        }

        private object[] ExecuteSelect(RequestSelect select, IDictionary<string, object[]> results) {
            object[] res = null;

            var whereValue = select.Where.Value;
            WhereType whereType = GetWhereType(whereValue);

            switch (select.From) {
                case "EventMetadata":
                    switch (whereType) {
                        case  WhereType.Region:
                            string city = whereValue.City;
                            string country = whereValue.Country;
                            string state = whereValue.State;

                            var metaData = _eventMetadataRepository.Table.Where(md => md.RegionRecord.Country == country && md.RegionRecord.State == state && md.RegionRecord.City == city);

                            return metaData.ToArray();
                    }
                    
                    break;
                case "Entity":
                    switch (whereType)
                    {
                        case WhereType.PreviousSelect:
                            var previousSelect = (EventMetadataRecord[])results[(string)select.Where.Value.SelectName];
                            var entity = previousSelect.Select(emd => emd.HostRecord);
                            return entity.ToArray();
                    }
                    break;
                case "Show":
                    break;
            }


            return res;
        }

        private WhereType GetWhereType(dynamic where) {
            var res = where.Country;
            if(res!= null)
                return WhereType.Region;

            res = where.SelectName;
            if (res != null)
                return WhereType.PreviousSelect;

            return WhereType.ValueCheck;
        }
    }

    public enum WhereType {
        Region,
        PreviousSelect,
        ValueCheck
    }
}
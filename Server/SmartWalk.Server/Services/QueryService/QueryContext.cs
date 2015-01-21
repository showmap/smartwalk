using Orchard.Environment.Configuration;
using SmartWalk.Server.Models.DataContracts;
using SmartWalk.Server.Records;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.QueryService
{
    public class QueryContext
    {
        internal static void InstantiateContext(ShellSettings shellSettings)
        {
            if (Instance == null)
            {
                Instance = new QueryContext(shellSettings);
            }
        }

        internal static QueryContext Instance { get; private set; }

        private QueryContext(ShellSettings shellSettings)
        {
            DbPrefix =
                !string.IsNullOrEmpty(shellSettings.DataTablePrefix)
                    ? shellSettings.DataTablePrefix + "_"
                    : string.Empty;

            // TODO: To resolve EventMetadataRecord table name dynamicaly
            EventMetadataTable = "SmartWalk_Server_EventMetadataRecord";
            EventMetadataView = "SmartWalk_Server_EventMetadataExtended";

            InitializeProperties();
        }

        private void InitializeProperties()
        {
            EventMetadataId =
                Reflection<EventMetadataRecord>.GetProperty(p => p.Id).Name;
            EventMetadataHost =
                Reflection<EventMetadata>.GetProperty(p => p.Host).Name;
            EventMetadataIsDeleted =
                Reflection<EventMetadataRecord>.GetProperty(p => p.IsDeleted).Name;
            EventMetadataStatus =
                Reflection<EventMetadataRecord>.GetProperty(p => p.Status).Name;
            EventMetadataEntityRecord =
                Reflection<EventMetadataRecord>.GetProperty(p => p.EntityRecord).Name;
            EventMetadataTitle =
                Reflection<EventMetadataRecord>.GetProperty(p => p.Title).Name;
            EventMetadataDescription =
                Reflection<EventMetadataRecord>.GetProperty(p => p.Description).Name;
            EventMetadataPicture =
                Reflection<EventMetadataRecord>.GetProperty(p => p.Picture).Name;
            EventMetadataStartTime =
                Reflection<EventMetadataRecord>.GetProperty(p => p.StartTime).Name;
            EventMetadataEndTime =
                Reflection<EventMetadataRecord>.GetProperty(p => p.EndTime).Name;
            EventMetadataLatitude =
                Reflection<EventMetadataRecord>.GetProperty(p => p.Latitude).Name;
            EventMetadataLongitude =
                Reflection<EventMetadataRecord>.GetProperty(p => p.Longitude).Name;
            EventMetadataCombineType =
                Reflection<EventMetadataRecord>.GetProperty(p => p.CombineType).Name;
            EventMetadataVenueOrderType =
                Reflection<EventMetadataRecord>.GetProperty(p => p.VenueOrderType).Name;
            EventMetadataVenueTitleFormatType =
                Reflection<EventMetadataRecord>.GetProperty(p => p.VenueTitleFormatType).Name;
            EventMetadataShows =
                Reflection<EventMetadata>.GetProperty(p => p.Shows).Name;

            EventMetadataProperties = new[]
                {
                    EventMetadataId,
                    EventMetadataHost,
                    EventMetadataEntityRecord,
                    EventMetadataIsDeleted,
                    EventMetadataStatus,
                    EventMetadataTitle,
                    EventMetadataDescription,
                    EventMetadataPicture,
                    EventMetadataStartTime,
                    EventMetadataEndTime,
                    EventMetadataLatitude,
                    EventMetadataLongitude,
                    EventMetadataCombineType,
                    EventMetadataShows
                };

            EntityType =
                Reflection<Entity>.GetProperty(p => p.Type).Name;
            EntityName =
                Reflection<Entity>.GetProperty(p => p.Name).Name;
            EntityDescription =
                Reflection<Entity>.GetProperty(p => p.Description).Name;
            EntityPicture =
                Reflection<Entity>.GetProperty(p => p.Picture).Name;
            EntityContacts =
                Reflection<Entity>.GetProperty(p => p.Contacts).Name;
            EntityAddresses =
                Reflection<Entity>.GetProperty(p => p.Addresses).Name;

            EventVenueDetailVenue =
                Reflection<EventVenueDetail>.GetProperty(p => p.Venue).Name;
            EventVenueDetailEntityRecord =
                Reflection<EventEntityDetailRecord>.GetProperty(p => p.EntityRecord).Name;
            EventVenueDetailEvent =
                Reflection<EventVenueDetail>.GetProperty(p => p.Event).Name;
            EventVenueDetailEventMetadataRecord =
                Reflection<EventEntityDetailRecord>.GetProperty(p => p.EventMetadataRecord).Name;
            EventVenueDetailSortOrder =
                Reflection<EventVenueDetail>.GetProperty(p => p.SortOrder).Name;
            EventVenueDetailDescription =
                Reflection<EventVenueDetail>.GetProperty(p => p.Description).Name;

            ShowVenue =
                Reflection<Show>.GetProperty(p => p.Venue).Name;
            ShowEntityRecord =
                Reflection<ShowRecord>.GetProperty(p => p.EntityRecord).Name;
            ShowIsReference =
                Reflection<Show>.GetProperty(p => p.IsReference).Name;
            ShowTitle =
                Reflection<Show>.GetProperty(p => p.Title).Name;
            ShowDescription =
                Reflection<Show>.GetProperty(p => p.Description).Name;
            ShowStartTime =
                Reflection<Show>.GetProperty(p => p.StartTime).Name;
            ShowEndTime =
                Reflection<Show>.GetProperty(p => p.EndTime).Name;
            ShowPicture =
                Reflection<Show>.GetProperty(p => p.Picture).Name;
            ShowDetailsUrl =
                Reflection<Show>.GetProperty(p => p.DetailsUrl).Name;
        }

        public string DbPrefix { get; private set; }

        public string EventMetadataTable { get; private set; }
        public string EventMetadataView { get; private set; }

        public string EventMetadataId { get; private set; }
        public string EventMetadataHost { get; private set; }
        public string EventMetadataEntityRecord { get; private set; }
        public string EventMetadataEntityRecordId { get; private set; }
        public string EventMetadataIsDeleted { get; private set; }
        public string EventMetadataStatus { get; private set; }
        public string EventMetadataTitle { get; private set; }
        public string EventMetadataDescription { get; private set; }
        public string EventMetadataPicture { get; private set; }
        public string EventMetadataStartTime { get; private set; }
        public string EventMetadataEndTime { get; private set; }
        public string EventMetadataLatitude { get; private set; }
        public string EventMetadataLongitude { get; private set; }
        public string EventMetadataCombineType { get; private set; }
        public string EventMetadataVenueOrderType { get; private set; }
        public string EventMetadataVenueTitleFormatType { get; private set; }
        public string EventMetadataShows { get; private set; }
        public string[] EventMetadataProperties { get; private set; }

        public string EntityType { get; private set; }
        public string EntityName { get; private set; }
        public string EntityDescription { get; private set; }
        public string EntityPicture { get; private set; }
        public string EntityContacts { get; private set; }
        public string EntityAddresses { get; private set; }

        public string EventVenueDetailVenue { get; private set; }
        public string EventVenueDetailEntityRecord { get; private set; }
        public string EventVenueDetailEvent { get; private set; }
        public string EventVenueDetailEventMetadataRecord { get; private set; }
        public string EventVenueDetailSortOrder { get; private set; }
        public string EventVenueDetailDescription { get; private set; }

        public string ShowVenue { get; private set; }
        public string ShowEntityRecord { get; private set; }
        public string ShowIsReference { get; private set; }
        public string ShowTitle { get; private set; }
        public string ShowDescription { get; private set; }
        public string ShowStartTime { get; private set; }
        public string ShowEndTime { get; private set; }
        public string ShowPicture { get; private set; }
        public string ShowDetailsUrl { get; private set; }
    }
}
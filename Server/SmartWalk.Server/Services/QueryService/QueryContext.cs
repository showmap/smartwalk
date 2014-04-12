﻿using Orchard.Environment.Configuration;
using SmartWalk.Server.Models.DataContracts;
using SmartWalk.Server.Records;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.QueryService
{
    public class QueryContext
    {
        private static QueryContext _instance;

        internal static QueryContext GetContext(ShellSettings shellSettings)
        {
            return _instance ?? (_instance = new QueryContext(shellSettings));
        }

        private QueryContext(ShellSettings shellSettings)
        {
            DbPrefix =
                !string.IsNullOrEmpty(shellSettings.DataTablePrefix)
                    ? shellSettings.DataTablePrefix + "_"
                    : string.Empty;

            // TODO: To resolve EventMetadataRecord table name dynamicaly
            EventMetadataTable = "SmartWalk_Server_EventMetadataRecord";
            EventMetadataTableAlias = "EMR";

            InitializeProperties();
        }

        private void InitializeProperties()
        {
            EventMetadataId =
                Reflection<EventMetadataRecord>.GetProperty(p => p.Id).Name;
            EventMetadataHost =
                Reflection<EventMetadata>.GetProperty(p => p.Host).Name;
            EventMetadataEntityRecordId =
                Reflection<EventMetadataRecord>.GetProperty(p => p.EntityRecord).Name + "_Id";
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
            EventMetadataShows =
                Reflection<EventMetadata>.GetProperty(p => p.Shows).Name;

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

            ShowVenue =
                Reflection<Show>.GetProperty(p => p.Venue).Name;
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
        public string EventMetadataTableAlias { get; private set; }

        public string EventMetadataId { get; private set; }
        public string EventMetadataHost { get; private set; }
        public string EventMetadataEntityRecordId { get; private set; }
        public string EventMetadataTitle { get; private set; }
        public string EventMetadataDescription { get; private set; }
        public string EventMetadataPicture { get; private set; }
        public string EventMetadataStartTime { get; private set; }
        public string EventMetadataEndTime { get; private set; }
        public string EventMetadataLatitude { get; private set; }
        public string EventMetadataLongitude { get; private set; }
        public string EventMetadataCombineType { get; private set; }
        public string EventMetadataShows { get; private set; }

        public string EntityType { get; private set; }
        public string EntityName { get; private set; }
        public string EntityDescription { get; private set; }
        public string EntityPicture { get; private set; }
        public string EntityContacts { get; private set; }
        public string EntityAddresses { get; private set; }

        public string ShowVenue { get; private set; }
        public string ShowIsReference { get; private set; }
        public string ShowTitle { get; private set; }
        public string ShowDescription { get; private set; }
        public string ShowStartTime { get; private set; }
        public string ShowEndTime { get; private set; }
        public string ShowPicture { get; private set; }
        public string ShowDetailsUrl { get; private set; }
    }
}
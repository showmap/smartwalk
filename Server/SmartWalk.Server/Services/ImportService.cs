using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using SmartWalk.Server.Models;
using SmartWalk.Server.Models.XmlModel;
using SmartWalk.Server.Records;

namespace SmartWalk.Server.Services
{
    public class ImportService : IImportService
    {
        private const string XmlDataPath = "http://smartwalk.me/data/us/ca";

        private readonly IOrchardServices _orchardServices;

        private readonly IRepository<AddressRecord> _addressRepository;
        private readonly IRepository<ContactRecord> _contactRepository;
        private readonly IRepository<EntityRecord> _entityRepository;
        private readonly IRepository<EntityMappingRecord> _entityMappingRepository;
        private readonly IRepository<EventMappingRecord> _eventMappingRepository;
        private readonly IRepository<EventMetadataRecord> _eventMetadataRepository;
        private readonly IRepository<RegionRecord> _regionRepository;
        private readonly IRepository<ShowRecord> _showRepository;
        private readonly IRepository<StorageRecord> _storageRepository;

        public ImportService(   IOrchardServices orchardServices,
                                IRepository<AddressRecord> addressRepository, IRepository<ContactRecord> contactRepository, 
                                IRepository<EntityRecord> entityRepository, IRepository<EntityMappingRecord> entityMappingRepository, 
                                IRepository<EventMappingRecord> eventMappingRepository, IRepository<EventMetadataRecord> eventMetadataRepository, 
                                IRepository<RegionRecord> regionRepository, IRepository<ShowRecord> showRepository, IRepository<StorageRecord> storageRepository) {
            _orchardServices = orchardServices;

            _addressRepository = addressRepository;
            _contactRepository = contactRepository;
            _entityRepository = entityRepository;
            _entityMappingRepository = entityMappingRepository;
            _eventMappingRepository = eventMappingRepository;
            _eventMetadataRepository = eventMetadataRepository;
            _regionRepository = regionRepository;
            _showRepository = showRepository;
            _storageRepository = storageRepository;
        }

        public void ImportXmlData() {
            var location = ParseLocation("sfbay");

            var currentUser = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();

            #region Example of record creation
            //_entityRepository.Create(new EntityRecord
            //{
            //    Description = "Description",
            //    Name = "Name",
            //    Picture = "Picture",
            //    Type = (int)EntityType.Host,
            //    SmartWalkUserRecord = currentUser.Record
            //});
            #endregion
        }

        private static Location ParseLocation(string regionName)
        {
            using (var indexXmlReader =
                XmlReader.Create(string.Format("{0}/{1}/index.xml", XmlDataPath, regionName)))
            {
                var serializer = new XmlSerializer(typeof(Location));
                var location = (Location)serializer.Deserialize(indexXmlReader);

                location.Organizations = location.Organizations
                    .Select(org => ParseOrganization(regionName, org.Id))
                    .ToArray();

                return location;
            }
        }

        private static Organization ParseOrganization(string regionName, string orgId)
        {
            using (var indexXmlReader =
                XmlReader.Create(string.Format("{0}/{1}/{2}/index.xml", XmlDataPath, regionName, orgId)))
            {
                var serializer = new XmlSerializer(typeof(Organization));
                var organization = (Organization)serializer.Deserialize(indexXmlReader);

                organization.Events = organization.EventRefs
                    .Where(er => er.HasSchedule)
                    .Select(er => ParseEvent(regionName, orgId, er.DateObject))
                    .ToArray();

                return organization;
            }
        }

        private static Event ParseEvent(string regionName, string orgId, DateTime eventDate)
        {
            using (var indexXmlReader =
                XmlReader.Create(string.Format(
                    "{0}/{1}/{2}/events/{2}-{3}.xml",
                    XmlDataPath,
                    regionName,
                    orgId,
                    String.Format("{0:yyyy-MM-dd}", eventDate))))
            {
                var serializer = new XmlSerializer(typeof(Event));
                var eventObj = (Event)serializer.Deserialize(indexXmlReader);

                foreach (var show in eventObj.Venues
                    .Where(v => v.Shows != null)
                    .SelectMany(v => v.Shows)
                    .ToArray())
                {
                    show.ParseShowTime(eventDate);
                }

                return eventObj;
            }
        }
    }
}
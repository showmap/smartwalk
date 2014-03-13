using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using SmartWalk.Server.Models;
using SmartWalk.Server.Models.XmlModel;
using SmartWalk.Server.Records;

namespace SmartWalk.Server.Services.ImportService
{
    [UsedImplicitly]
    public class ImportService : IImportService
    {
        private const string XmlDataPath = "http://smartwalk.me/data/us/ca";

        private readonly IOrchardServices _orchardServices;

        private readonly IRepository<AddressRecord> _addressRepository;
        private readonly IRepository<ContactRecord> _contactRepository;
        private readonly IRepository<EntityRecord> _entityRepository;
        private readonly IRepository<EventMappingRecord> _eventMappingRepository;
        private readonly IRepository<EventMetadataRecord> _eventMetadataRepository;
        private readonly IRepository<RegionRecord> _regionRepository;
        private readonly IRepository<ShowRecord> _showRepository;
        private readonly IRepository<StorageRecord> _storageRepository;

        private List<string> _log;

        public ImportService(
            IOrchardServices orchardServices,
            IRepository<AddressRecord> addressRepository, 
            IRepository<ContactRecord> contactRepository, 
            IRepository<EntityRecord> entityRepository, 
            IRepository<EventMappingRecord> eventMappingRepository, 
            IRepository<EventMetadataRecord> eventMetadataRepository, 
            IRepository<RegionRecord> regionRepository, 
            IRepository<ShowRecord> showRepository, 
            IRepository<StorageRecord> storageRepository)
        {
            _orchardServices = orchardServices;
            _addressRepository = addressRepository;
            _contactRepository = contactRepository;
            _entityRepository = entityRepository;
            _eventMappingRepository = eventMappingRepository;
            _eventMetadataRepository = eventMetadataRepository;
            _regionRepository = regionRepository;
            _showRepository = showRepository;
            _storageRepository = storageRepository;
        }

        public void ImportXmlData(List<string> log)
        {
            _log = log;
            _log.Add(string.Format(
                "Importing production XML data from {0} at {1}", 
                XmlDataPath, 
                DateTime.UtcNow));

            var location = ParseLocation("sfbay");

            ImportLocation(location);

            _log = null;
        }

        #region XML parsing

        private Location ParseLocation(string regionName)
        {
            var url = string.Format("{0}/{1}/index.xml", XmlDataPath, regionName);

            try
            {
                using (var indexXmlReader = XmlReader.Create(url))
                {
                    var serializer = new XmlSerializer(typeof(Location));
                    var location = (Location)serializer.Deserialize(indexXmlReader);

                    location.Organizations = location.Organizations
                        .Select(org => ParseOrganization(regionName, org.Id))
                        .ToArray();

                    _log.Add(string.Format("Location {0} parsing finished", location.Name));

                    return location;
                }
            }
            catch (WebException)
            {
                _log.Add(string.Format("Error loading from: {0}", url));
                throw;
            }
        }

        private Organization ParseOrganization(string regionName, string orgId)
        {
            var url = string.Format("{0}/{1}/{2}/index.xml", XmlDataPath, regionName, orgId);

            try
            {
                using (var indexXmlReader = XmlReader.Create(url))
                {
                    var serializer = new XmlSerializer(typeof(Organization));
                    var organization = (Organization)serializer.Deserialize(indexXmlReader);

                    organization.Events = organization.EventRefs
                        .Where(er => er.HasSchedule)
                        .Select(er => ParseEvent(regionName, orgId, er.DateObject))
                        .OrderBy(e => e.StartDateObject)
                        .ToArray();

                    _log.Add(string.Format("Organization {0} parsing finished", organization.Name));

                    return organization;
                }
            }
            catch (WebException)
            {
                _log.Add(string.Format("Error loading from: {0}", url));
                throw;
            }
        }

        private Event ParseEvent(string regionName, string orgId, DateTime eventDate)
        {
            var url = string.Format(
                "{0}/{1}/{2}/events/{2}-{3}.xml",
                XmlDataPath,
                regionName,
                orgId,
                String.Format("{0:yyyy-MM-dd}", eventDate));

            try
            {
                using (var indexXmlReader = XmlReader.Create(url))
                {
                    var serializer = new XmlSerializer(typeof(Event));
                    var eventObj = (Event)serializer.Deserialize(indexXmlReader);

                    eventObj.StartDateObject = eventDate;

                    foreach (var show in eventObj.Venues
                        .Where(v => v.Shows != null)
                        .SelectMany(v => v.Shows)
                        .ToArray())
                    {
                        show.ParseShowTime(eventDate);
                    }

                    _log.Add(string.Format("Event {0} parsing finished", eventObj.StartDate));

                    return eventObj;
                }
            }
            catch (WebException)
            {
                _log.Add(string.Format("Error loading from: {0}", url));
                throw;
            }
        }

        #endregion

        #region Importing

        private void ImportLocation(Location location)
        {
            var currentUser = _orchardServices.WorkContext.CurrentUser.As<SmartWalkUserPart>();
            if (currentUser == null)
            {
                throw new InvalidOperationException("Current user is not defined!");
            }

            #region Oakland Region

            var regionName = string.Format("{0}, {1}, {2}", "United States", "California", "Oakland");
            var oaklandRegion = _regionRepository.Get(reg => reg.Region == regionName);
            if (oaklandRegion == null)
            {
                oaklandRegion = new RegionRecord
                    {
                        Region = regionName,
                        Latitude = 0,
                        Longitude = 0,
                    };
                _regionRepository.Create(oaklandRegion);
                _regionRepository.Flush();
                _log.Add("Oakland region created");
            }
            #endregion

            #region San Francisco Region
            regionName = string.Format("{0}, {1}, {2}", "United States", "California", "San Francisco");
            var sfRegion = _regionRepository.Get(reg => reg.Region == regionName);
            if (sfRegion == null)
            {
                sfRegion = new RegionRecord
                    {
                        Region = regionName,
                        Latitude = 0,
                        Longitude = 0
                    };
                _regionRepository.Create(sfRegion);
                _regionRepository.Flush();
                _log.Add("San Francisco region created");
            }
            #endregion

            #region SmartWalk Data Storage
            var storage = _storageRepository.Get(stor => stor.StorageKey == StorageKeys.SmartWalk);
            if (storage == null)
            {
                storage = new StorageRecord
                    {
                        StorageKey = StorageKeys.SmartWalk,
                        Description = "SmartWalk Data Storage"
                    };
                _storageRepository.Create(storage);
                _storageRepository.Flush();
                _log.Add("SmartWalk storge record created");
            }
            #endregion

            if (location != null && location.Organizations != null)
            {
                foreach (var xmlOrg in location.Organizations)
                {
                    var hostRegion = xmlOrg.Id == "oam" ? oaklandRegion : sfRegion;
                    var hostEntity =
                        CreateOrUpdateEntity(
                            xmlOrg,
                            EntityType.Host,
                            hostRegion,
                            currentUser.Record);

                    if (xmlOrg.Events != null)
                    {
                        foreach (var xmlOrgEvent in xmlOrg.Events)
                        {
                            var eventMetadata =
                                CreateEventMetadata(
                                    xmlOrgEvent,
                                    hostRegion,
                                    hostEntity,
                                    currentUser.Record);

                            if (xmlOrgEvent.Venues != null)
                            {
                                foreach (var xmlVenue in xmlOrgEvent.Venues)
                                {
                                    var venueEntity =
                                        CreateOrUpdateEntity(
                                            xmlVenue,
                                            EntityType.Venue,
                                            hostRegion,
                                            currentUser.Record);

                                    CreateOrUpdateShows(
                                        storage,
                                        eventMetadata,
                                        venueEntity,
                                        xmlVenue.Shows);
                                }
                            }
                        }
                    }
                }
            }
        }

        private EntityRecord CreateOrUpdateEntity(
            IEntity xmlEntity, 
            EntityType type, 
            RegionRecord region,
            SmartWalkUserRecord user)
        {
            var result = _entityRepository.Get(ent => ent.Name == xmlEntity.Name);
            if (result == null)
            {
                result = new EntityRecord
                    {
                        Name = xmlEntity.Name,
                        Type = (int)type,
                        SmartWalkUserRecord = user
                    };

                _entityRepository.Create(result);
                _log.Add(string.Format("{0} entity created", result.Name));
            }

            result.Description = xmlEntity.Description;
            result.Picture = xmlEntity.Logo;

            _entityRepository.Update(result);
            _entityRepository.Flush();
            _log.Add(string.Format("{0} entity updated", result.Name));

            CreateContacts(result, xmlEntity.Contacts);
            CreateOrUpdateAddresses(result, xmlEntity.Addresses, region);

            return result;
        }

        private void CreateContacts(EntityRecord entity, IEnumerable<Contact> xmlContacts)
        {
            if (xmlContacts == null) return;

            var contacts = _contactRepository
                .Fetch(cont => cont.EntityRecord == entity)
                .ToArray();
            foreach (var xmlContact in xmlContacts)
            {
                var phone = xmlContact as Phone;
                if (phone != null)
                {
                    CreateContact(contacts, entity, ContactType.Phone, phone.Name, phone.Value);
                }

                var email = xmlContact as Email;
                if (email != null)
                {
                    CreateContact(contacts, entity, ContactType.Email, email.Name, email.Value);
                }

                var web = xmlContact as Web;
                if (web != null)
                {
                    CreateContact(contacts, entity, ContactType.Url, null, web.Value);
                }
            }
        }

        private void CreateContact(
            IEnumerable<ContactRecord> contacts, 
            EntityRecord entity,
            ContactType type, 
            string title, 
            string value)
        {
            if (!contacts.Any(cont => 
                    cont.Type == (int)type && 
                    cont.Title == title &&
                    cont.Contact == value))
            {
                var contact = new ContactRecord
                    {
                        EntityRecord = entity,
                        Type = (int)type,
                        Title = title,
                        Contact = value
                    };
                _contactRepository.Create(contact);
                _contactRepository.Flush();
                _log.Add(string.Format("{0} contact created", contact.Contact));
            }
        }

        private void CreateOrUpdateAddresses(
            EntityRecord entity, 
            IEnumerable<Address> xmlAddresses,
            RegionRecord region)
        {
            if (xmlAddresses == null) return;

            var addresses = _addressRepository
                .Fetch(addr => addr.EntityRecord == entity)
                .ToArray();
            foreach (var xmlAddress in xmlAddresses)
            {
                var address = addresses.FirstOrDefault(addr =>
                    addr.Address == xmlAddress.Text);
                if (address == null)
                {
                    address = new AddressRecord
                        {
                            EntityRecord = entity,
                            Address = xmlAddress.Text
                        };
                    _addressRepository.Create(address);
                    _log.Add(string.Format("{0} address created", address.Address));
                }

                address.Latitude = xmlAddress.Latitude;
                address.Longitude = xmlAddress.Longitude;

                _addressRepository.Update(address);
                _addressRepository.Flush();
                _log.Add(string.Format("{0} address updated", address.Address));
            }
        }

        private EventMetadataRecord CreateEventMetadata(
            Event xmlOrgEvent,
            RegionRecord region,
            EntityRecord hostEntity,
            SmartWalkUserRecord user)
        {
            var result = _eventMetadataRepository
                .Get(evMet => evMet.StartTime == xmlOrgEvent.StartDateObject.Date);
            if (result == null)
            {
                result = new EventMetadataRecord
                    {
                        RegionRecord = region,
                        EntityRecord = hostEntity,
                        StartTime = xmlOrgEvent.StartDateObject.Date,
                        CombineType = (int)CombineType.None,
                        SmartWalkUserRecord = user,
                        DateCreated = xmlOrgEvent.StartDateObject.Date,
                        DateModified = DateTime.UtcNow
                    };
                
                _eventMetadataRepository.Create(result);
                _eventMetadataRepository.Flush();
                _log.Add(string.Format("{0} event metadata created", result.Title));
            }

            return result;
        }

        private void CreateOrUpdateShows(
            StorageRecord storage,
            EventMetadataRecord eventMetadata,
            EntityRecord venue,
            Show[] xmlShows)
        {
            if (xmlShows == null) return;

            var shows = eventMetadata.ShowRecords;

            if (xmlShows.Any())
            {
                foreach (var xmlShow in xmlShows)
                {
                    var show = shows.FirstOrDefault(s =>
                        s.EntityRecord == venue &&
                        s.Description == xmlShow.Desciption);
                    if (show == null)
                    {
                        show = new ShowRecord
                            {
                                EntityRecord = venue,
                                EventMetadataRecord = eventMetadata,
                                Description = xmlShow.Desciption
                            };
                        _showRepository.Create(show);
                        _log.Add(string.Format("{0} show created", show.Description));
                    }

                    show.StartTime = xmlShow.StartTimeObject;
                    show.EndTime = xmlShow.EndTimeObject;
                    show.Picture = xmlShow.Logo;
                    show.DetailsUrl = xmlShow.Web;

                    _showRepository.Update(show);
                    _showRepository.Flush();
                    _log.Add(string.Format("{0} show updated", show.Description));
                }
            }
            else
            {
                var refShow = shows.FirstOrDefault(s =>
                        s.EntityRecord == venue &&
                        s.IsReference);
                if (refShow == null)
                {
                    refShow = new ShowRecord
                        {
                            EntityRecord = venue,
                            EventMetadataRecord = eventMetadata,
                            IsReference = true
                        };
                    _showRepository.Create(refShow);
                    _showRepository.Flush();
                    _log.Add(string.Format("Reference show created"));                    
                }
            }
        }

        #endregion
    }
}
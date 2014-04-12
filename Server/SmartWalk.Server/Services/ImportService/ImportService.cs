using System;
using System.Collections.Generic;
using System.Drawing;
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
using SmartWalk.Server.Utils;
using SmartWalk.Shared.Utils;

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
        private readonly IRepository<EventMetadataRecord> _eventMetadataRepository;
        private readonly IRepository<ShowRecord> _showRepository;

        private List<string> _log;

        public ImportService(
            IOrchardServices orchardServices,
            IRepository<AddressRecord> addressRepository, 
            IRepository<ContactRecord> contactRepository, 
            IRepository<EntityRecord> entityRepository, 
            IRepository<EventMetadataRecord> eventMetadataRepository, 
            IRepository<ShowRecord> showRepository)
        {
            _orchardServices = orchardServices;
            _addressRepository = addressRepository;
            _contactRepository = contactRepository;
            _entityRepository = entityRepository;
            _eventMetadataRepository = eventMetadataRepository;
            _showRepository = showRepository;
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

            if (location != null && location.Organizations != null)
            {
                foreach (var xmlOrg in location.Organizations)
                {
                    var hostEntity =
                        CreateOrUpdateEntity(
                            xmlOrg,
                            EntityType.Host,
                            currentUser.Record);

                    if (xmlOrg.Events != null)
                    {
                        foreach (var xmlOrgEvent in xmlOrg.Events)
                        {
                            var eventMetadata =
                                CreateEventMetadata(
                                    xmlOrgEvent,
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
                                            currentUser.Record);
                                    CreateOrUpdateShows(
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
            SmartWalkUserRecord user)
        {
            var result = _entityRepository.Get(ent => ent.Name == xmlEntity.Name);
            if (result == null)
            {
                result = new EntityRecord
                    {
                        Name = xmlEntity.Name.TrimIt(),
                        Type = (int)type,
                        SmartWalkUserRecord = user,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    };

                _entityRepository.Create(result);
                _log.Add(string.Format("{0} entity created", result.Name));
            }

            result.Description = xmlEntity.Description.TrimIt();
            result.Picture = xmlEntity.Logo;

            _entityRepository.Update(result);
            _entityRepository.Flush();
            _log.Add(string.Format("{0} entity updated", result.Name));

            CreateContacts(result, xmlEntity.Contacts);
            CreateOrUpdateAddresses(result, xmlEntity.Addresses);

            return result;
        }

        private void CreateContacts(EntityRecord entity, IEnumerable<Contact> xmlContacts)
        {
            if (xmlContacts == null) return;

            var contacts = _contactRepository
                .Fetch(cont => cont.EntityRecord.Id == entity.Id)
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
                        Title = title.TrimIt(),
                        Contact = value.TrimIt()
                    };
                _contactRepository.Create(contact);
                _contactRepository.Flush();
                _log.Add(string.Format("{0} contact created", contact.Contact));
            }
        }

        private void CreateOrUpdateAddresses(
            EntityRecord entity, 
            IEnumerable<Address> xmlAddresses)
        {
            if (xmlAddresses == null) return;

            var addresses = _addressRepository
                .Fetch(addr => addr.EntityRecord.Id == entity.Id)
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
                            Address = xmlAddress.Text.TrimIt()
                        };
                    _addressRepository.Create(address);
                    _log.Add(string.Format("{0} address created", address.Address));
                }

                address.Latitude = xmlAddress.Latitude;
                address.Longitude = xmlAddress.Longitude;
                address.Tip = xmlAddress.Tip;

                _addressRepository.Update(address);
                _addressRepository.Flush();
                _log.Add(string.Format("{0} address updated", address.Address));
            }
        }

        private EventMetadataRecord CreateEventMetadata(
            Event xmlOrgEvent,
            EntityRecord hostEntity,
            SmartWalkUserRecord user)
        {
            var result = _eventMetadataRepository
                .Get(evMet => 
                    evMet.EntityRecord.Id == hostEntity.Id &&
                    evMet.StartTime == xmlOrgEvent.StartDateObject.Date);
            if (result == null)
            {
                result = new EventMetadataRecord
                    {
                        EntityRecord = hostEntity,
                        StartTime = xmlOrgEvent.StartDateObject.Date,
                        CombineType = (int)CombineType.None,
                        SmartWalkUserRecord = user,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow,
                        IsPublic = true
                    };
                
                _eventMetadataRepository.Create(result);
                _log.Add(string.Format("{0} event metadata created", result.Title));
            }

            result.Title = xmlOrgEvent.Title;
            result.Description = xmlOrgEvent.Description;
            result.Picture = xmlOrgEvent.Logo;
            SetEventMetadataCoordinate(result, xmlOrgEvent.Venues);

            _eventMetadataRepository.Update(result);
            _eventMetadataRepository.Flush();
            _log.Add(string.Format(
                "{0} event metadata coordinates updated to ({1}, {2})", 
                result.Title, 
                result.Latitude, 
                result.Longitude));

            return result;
        }

        private static void SetEventMetadataCoordinate(
            EventMetadataRecord eventMetadata,
            IEnumerable<Venue> venues)
        {
            var coordinates = venues.SelectMany(
                v =>
                v.Addresses != null
                    ? v.Addresses.Select(
                        a => new PointF((float)a.Latitude, (float)a.Longitude))
                    : Enumerable.Empty<PointF>()).ToArray();

            var middle = MapUtil.GetMiddleCoordinate(coordinates);

            eventMetadata.Latitude = middle.X;
            eventMetadata.Longitude = middle.Y;
        }

        private void CreateOrUpdateShows(
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
                    string title;
                    string description = null;
                    var xmlDescription = xmlShow.Desciption.TrimIt();

                    if (xmlDescription.Length >= 255)
                    {
                        title = xmlDescription.Substring(0, 50);
                        description = xmlDescription;
                    }
                    else
                    {
                        title = xmlDescription;
                    }

                    var show = shows.FirstOrDefault(s =>
                        s.EntityRecord.Id == venue.Id &&
                        s.Title == title);
                    if (show == null)
                    {
                        show = new ShowRecord
                            {
                                EntityRecord = venue,
                                EventMetadataRecord = eventMetadata,
                                Title = title,
                                Description = description,
                                IsDeleted = false,
                                DateCreated = DateTime.UtcNow,
                                DateModified = DateTime.UtcNow,
                            };
                        _showRepository.Create(show);
                        _log.Add(string.Format("{0} show created", show.Title));
                    }

                    show.StartTime = xmlShow.StartTimeObject;
                    show.EndTime = xmlShow.EndTimeObject;
                    show.Picture = xmlShow.Logo.TrimIt();
                    show.DetailsUrl = xmlShow.Web.TrimIt();

                    _showRepository.Update(show);
                    _showRepository.Flush();
                    _log.Add(string.Format("{0} show updated", show.Title));
                }
            }
            else
            {
                var refShow = shows.FirstOrDefault(s =>
                        s.EntityRecord.Id == venue.Id &&
                        s.IsReference);
                if (refShow == null)
                {
                    refShow = new ShowRecord
                        {
                            EntityRecord = venue,
                            EventMetadataRecord = eventMetadata,
                            IsReference = true,
                            IsDeleted = false,
                            DateCreated = DateTime.UtcNow,
                            DateModified = DateTime.UtcNow,
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
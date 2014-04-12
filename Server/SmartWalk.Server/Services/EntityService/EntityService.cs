using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.CultureService;
using SmartWalk.Server.ViewModels;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.EntityService
{
    public class EntityService : IEntityService
    {
        private readonly IRepository<ContactRecord> _contactRepository;
        private readonly IRepository<EntityRecord> _entityRepository;
        private readonly IRepository<AddressRecord> _addressRepository;
        private readonly IRepository<ShowRecord> _showRepository;
        private readonly IRepository<EventMetadataRecord> _metadataRepository;

        private readonly Lazy<CultureInfo> _cultureInfo;

        public EntityService(ICultureService cultureService, 
            IRepository<ContactRecord> contactRepository, IRepository<EntityRecord> entityRepository, 
            IRepository<AddressRecord> addressRepository, IRepository<ShowRecord> showRepository, IRepository<EventMetadataRecord> metadataRepository) {
            _metadataRepository = metadataRepository;
            _showRepository = showRepository;
            _entityRepository = entityRepository;
            _contactRepository = contactRepository;
            _addressRepository = addressRepository;

            _cultureInfo = new Lazy<CultureInfo>(cultureService.GetCurrentCulture);
        }

        #region Shows

        public ShowVm GetShow(int showId) {
            var show = _showRepository.Get(showId);
            return show == null ? null : ViewModelContractFactory.CreateViewModelContract(show);
        }

        public void DeleteShow(int showId) {
            var show = _showRepository.Get(showId);

            if (show == null)
                return;

            _showRepository.Delete(show);
            _showRepository.Flush();

            CheckShowVenue(show.EventMetadataRecord.Id, show.EntityRecord.Id);
        }


        public void DeleteEventVenue(EntityVm item) {
            if (item.AllShows.Count == 0)
                return;

            var shows = _showRepository.Table.Where(s => s.EventMetadataRecord.Id == item.AllShows.First().EventMetedataId && s.EntityRecord.Id == item.Id).ToList();

            foreach (var showRecord in shows) {
                showRecord.IsDeleted = true;
                _showRepository.Flush();
            }
        }

        public ShowVm SaveEventVenue(EntityVm item) {
            foreach (var showVm in item.AllShows) {
                SaveOrAddShow(showVm);
            }

            return CheckShowVenue(item.EventMetedataId, item.Id);
        }

        private ShowVm CheckShowVenue(int eventId, int venueId)
        {
            var shows = _showRepository.Table.Where(s => s.EventMetadataRecord.Id == eventId && s.EntityRecord.Id == venueId).ToList();
            if (shows.Any()) {
                if (shows.Count(s => !s.IsReference) > 0 && shows.Count(s => s.IsReference) > 0) {
                    foreach (var showRecord in shows.Where(s => s.IsReference)) {
                        _showRepository.Delete(showRecord);
                        _showRepository.Flush();
                    }
                }                
            }
            else {
                var show = new ShowRecord
                {
                    EventMetadataRecord = _metadataRepository.Get(eventId),
                    EntityRecord = _entityRepository.Get(venueId),
                    IsReference = true,
                    IsDeleted = false,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                };
                _showRepository.Create(show);
                _showRepository.Flush();

                return ViewModelContractFactory.CreateViewModelContract(show);
            }

            return null;
        }


        public ShowVm SaveOrAddShow(ShowVm item)
        {
            var show = _showRepository.Get(item.Id);
            var metadata = _metadataRepository.Get(item.EventMetedataId);
            var venue = _entityRepository.Get(item.VenueId);

            if (metadata == null || venue == null)
                return null;

            var dtFrom = item.StartDateTime.ParseDateTime(_cultureInfo.Value);
            var dtTo = item.EndDateTime.ParseDateTime(_cultureInfo.Value);

            if (show == null)
            {
                show = new ShowRecord
                {
                    EventMetadataRecord = metadata,
                    EntityRecord = venue,
                    IsReference = false,
                    Title = item.Title,
                    Description = item.Description,
                    StartTime = dtFrom,
                    EndTime = dtTo,
                    Picture = item.Picture,
                    DetailsUrl = item.DetailsUrl,
                    IsDeleted = false,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                };

                _showRepository.Create(show);
                _showRepository.Flush();

                CheckShowVenue(metadata.Id, venue.Id);
            }
            else {
                show.EntityRecord = venue;
                show.Title = item.Title;
                show.Description = item.Description;
                show.StartTime = dtFrom;
                show.EndTime = dtTo;
                show.Picture = item.Picture;
                show.DetailsUrl = item.DetailsUrl;
                show.DateModified = DateTime.Now;

                _showRepository.Flush();
            }

            return ViewModelContractFactory.CreateViewModelContract(show);
        }

        
        #endregion

        #region Entities
        public IList<EntityVm> GetUserEntities(SmartWalkUserRecord user, EntityType type) {
            return user.Entities.Where(e => e.Type == (int) type && !e.IsDeleted).Select(ViewModelContractFactory.CreateViewModelContract).ToList();
        }

        public EntityVm GetEntityVmById(int hostId, EntityType type) {
            var entity = _entityRepository.Get(hostId);

            return entity == null ? new EntityVm {Id = 0, Type = (int) type} : GetEntityVm(entity);
        }

        public EntityVm GetEntityVm(EntityRecord entity) {
            return ViewModelContractFactory.CreateViewModelContract(entity);
        }

        public IList<EntityVm> GetEventEntities(EventMetadataRecord metadata) {
            return _entityRepository.Table.Where(e => e.Type == (int) EntityType.Venue && !e.IsDeleted).Select(e => ViewModelContractFactory.CreateViewModelContract(e, metadata)).ToList();
        }

        public EntityVm SaveOrAddEntity(SmartWalkUserRecord user, EntityVm entityVm) {
            var entity = _entityRepository.Get(entityVm.Id);

            if (entity == null)
            {
                entity = new EntityRecord
                {
                    Name = entityVm.Name,
                    Type = entityVm.Type,
                    SmartWalkUserRecord = user,
                    Picture = entityVm.Picture,
                    Description = entityVm.Description,
                    IsDeleted = false,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now
                };

                _entityRepository.Create(entity);
            }
            else {
                entity.Picture = entityVm.Picture;
                entity.Description = entityVm.Description;
                entity.DateModified = DateTime.Now;
            }
            
            _entityRepository.Flush();
            
            foreach (var contact in entityVm.AllContacts) {
                contact.EntityId = entity.Id;

                if (contact.State == VmItemState.Deleted)
                    DeleteContact(contact.Id);
                else
                    entity.ContactRecords.Add(SaveOrAddContactInner(contact));
            }

            foreach (var address in entityVm.AllAddresses) {
                address.EntityId = entity.Id;

                if (address.State == VmItemState.Deleted)
                    DeleteAddress(address.Id);
                else
                    entity.AddressRecords.Add(SaveOrAddAddressInner(address));
            }

            return GetEntityVm(entity);
        }

        public void DeleteEntity(int entityId) {
            var entity = _entityRepository.Get(entityId);

            if (entity == null)
                return;

            entity.IsDeleted = true;
            _entityRepository.Flush();
        }
        #endregion

        #region Addresses
        public AddressVm SaveOrAddAddress(AddressVm addressVm)
        {
            return ViewModelContractFactory.CreateViewModelContract(SaveOrAddAddressInner(addressVm));
        }

        private AddressRecord SaveOrAddAddressInner(AddressVm addressVm)
        {
            var address = _addressRepository.Get(addressVm.Id);

            if (address == null) {
                var entity = _entityRepository.Get(addressVm.EntityId);

                address = new AddressRecord
                {
                    EntityRecord = entity,
                    Address = addressVm.Address,
                    Tip = addressVm.Tip,
                    Latitude = addressVm.Latitude,
                    Longitude = addressVm.Longitude
                };

                _addressRepository.Create(address);
            }
            else
            {
                address.Address = addressVm.Address;
                address.Tip = addressVm.Tip;
                address.Latitude = addressVm.Latitude;
                address.Longitude = addressVm.Longitude;
            }

            _addressRepository.Flush();

            return address;
        }

        public void DeleteAddress(int addressId)
        {
            var address = _addressRepository.Get(addressId);

            if (address == null)
                return;

            _addressRepository.Delete(address);
            _addressRepository.Flush();
        }

        public AddressVm GetAddress(int addressId)
        {
            var address = _addressRepository.Get(addressId);
            return address == null ? null : ViewModelContractFactory.CreateViewModelContract(address);
        }
        #endregion

        #region Contacts
        public ContactVm GetContact(int contactId)
        {
            var contact = _contactRepository.Get(contactId);
            return contact == null ? null : ViewModelContractFactory.CreateViewModelContract(contact); 
        }

        public ContactVm SaveOrAddContact(ContactVm contactVm)
        {
            return ViewModelContractFactory.CreateViewModelContract(SaveOrAddContactInner(contactVm));
        }

        private ContactRecord SaveOrAddContactInner(ContactVm contactVm) {
            var contact = _contactRepository.Get(contactVm.Id);

            if (contact == null) {
                var entity = _entityRepository.Get(contactVm.EntityId);

                contact = new ContactRecord
                {
                    EntityRecord = entity,
                    Type = contactVm.Type,
                    Title = contactVm.Title,
                    Contact = contactVm.Contact
                };

                _contactRepository.Create(contact);
            }
            else
            {
                contact.Title = contactVm.Title;
                contact.Contact = contactVm.Contact;
            }

            _contactRepository.Flush();

            return contact;
        }

        public void DeleteContact(int contactId)
        {
            var contact = _contactRepository.Get(contactId);

            if (contact == null)
                return;

            _contactRepository.Delete(contact);
            _contactRepository.Flush();            
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.CultureService;
using SmartWalk.Server.ViewModels;

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
        public void DeleteShow(ShowVm item) {
            var show = _showRepository.Get(item.Id);

            if (show == null)
                return;

            _showRepository.Delete(show);
            _showRepository.Flush();

            CheckIsReferenceShow(item);
        }


        public void DeleteEventVenue(EntityVm item) {
            if (item.AllShows.Count == 0)
                return;

            var shows = _showRepository.Table.Where(s => s.EventMetadataRecord.Id == item.AllShows.First().EventMetedataId && s.EntityRecord.Id == item.Id).ToList();

            foreach (var showRecord in shows) {
                _showRepository.Delete(showRecord);
                _showRepository.Flush();
            }
        }

        private void CheckIsReferenceShow(ShowVm item) {
            var shows = _showRepository.Table.Where(s => s.EventMetadataRecord.Id == item.EventMetedataId && s.EntityRecord.Id == item.VenueId).ToList();
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
                    EventMetadataRecord = _metadataRepository.Get(item.EventMetedataId),
                    EntityRecord = _entityRepository.Get(item.VenueId),
                    IsReference = true,
                };
                _showRepository.Create(show);
                _showRepository.Flush();
            }
        }


        public ShowVm SaveOrAddShow(ShowVm item)
        {
            var show = _showRepository.Get(item.Id);
            var metadata = _metadataRepository.Get(item.EventMetedataId);
            var venue = _entityRepository.Get(item.VenueId);

            if (metadata == null || venue == null)
                return null;

            var dtFrom = ParseDateTime(item.StartDateTime);
            var dtTo = ParseDateTime(item.EndDateTime);

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
                    DetailsUrl = item.DetailsUrl
                };

                _showRepository.Create(show);
                _showRepository.Flush();

                CheckIsReferenceShow(item);
            }
            else
            {
                show.Title = item.Title;
                show.Description = item.Description;
                show.StartTime = dtFrom;
                show.EndTime = dtTo;
                show.Picture = item.Picture;
                show.DetailsUrl = item.DetailsUrl;

                _showRepository.Flush();
            }

            return ViewModelContractFactory.CreateViewModelContract(show);
        }

        private DateTime? ParseDateTime(string dtValue) {
            DateTime dtParse;

            if (DateTime.TryParse(dtValue, _cultureInfo.Value, DateTimeStyles.None, out dtParse))
                return dtParse;

            return null;
        }
        #endregion

        #region Entities
        public IList<EntityVm> GetUserEntities(SmartWalkUserRecord user, EntityType type) {
            return user.Entities.Where(e => e.Type == (int) type).Select(ViewModelContractFactory.CreateViewModelContract).ToList();
        }

        public EntityVm GetEntityVmById(int hostId, EntityType type) {
            var entity = _entityRepository.Get(hostId);

            return entity == null ? new EntityVm {Id = 0, Type = (int) type} : GetEntityVm(entity);
        }

        public EntityVm GetEntityVm(EntityRecord entity) {
            return ViewModelContractFactory.CreateViewModelContract(entity);
        }

        public IList<EntityVm> GetEventEntities(EventMetadataRecord metadata) {
            return _entityRepository.Table.Where(e => e.ShowRecords.Any(s => s.EventMetadataRecord.Id == metadata.Id)).Select(e => ViewModelContractFactory.CreateViewModelContract(e, metadata)).ToList();
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
                };

                _entityRepository.Create(entity);
            }
            else {
                entity.Picture = entityVm.Picture;
                entity.Description = entityVm.Description;
            }
            
            _entityRepository.Flush();
            
            foreach (var contact in entityVm.AllContacts)
            {
                if (contact.State == VmItemState.Deleted)
                    DeleteContact(contact.Id);
                else
                    entity.ContactRecords.Add(SaveOrAddContactInner(entity, contact));
            }

            foreach (var address in entityVm.AllAddresses) {
                if (address.State == VmItemState.Deleted)
                    DeleteAddress(address.Id);
                else
                    entity.AddressRecords.Add(SaveOrAddAddressInner(entity, address));
            }

            return GetEntityVm(entity);
        }

        public void DeleteEntity(int entityId) {
            var entity = _entityRepository.Get(entityId);

            if (entity == null)
                return;

            foreach (var contact in entity.ContactRecords) {
                _contactRepository.Delete(contact); 
                _contactRepository.Flush();
            }

            foreach (var address in entity.AddressRecords)
            {
                _addressRepository.Delete(address);
                _addressRepository.Flush();
            }

            _entityRepository.Delete(entity);
            _entityRepository.Flush();
        }
        #endregion

        #region Addresses
        public AddressVm SaveOrAddAddress(EntityRecord entity, AddressVm addressVm)
        {
            return ViewModelContractFactory.CreateViewModelContract(SaveOrAddAddressInner(entity, addressVm));
        }

        private AddressRecord SaveOrAddAddressInner(EntityRecord entity, AddressVm addressVm)
        {
            var address = _addressRepository.Get(addressVm.Id);

            if (address == null)
            {

                address = new AddressRecord
                {
                    EntityRecord = entity,
                    Address = addressVm.Address,
                    Latitude = addressVm.Latitude,
                    Longitude = addressVm.Longitude
                };

                _addressRepository.Create(address);
            }
            else
            {
                address.Address = addressVm.Address;
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
        #endregion

        #region Contacts
        public ContactVm SaveOrAddContact(EntityRecord host, ContactVm contactVm)
        {
            return ViewModelContractFactory.CreateViewModelContract(SaveOrAddContactInner(host, contactVm));
        }

        private ContactRecord SaveOrAddContactInner(EntityRecord host, ContactVm contactVm) {
            var contact = _contactRepository.Get(contactVm.Id);

            if (contact == null)
            {

                contact = new ContactRecord
                {
                    EntityRecord = host,
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
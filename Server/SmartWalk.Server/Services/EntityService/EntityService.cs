using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;
using SmartWalk.Shared;

namespace SmartWalk.Server.Services.EntityService
{
    [UsedImplicitly]
    public class EntityService : IEntityService
    {
        private readonly IRepository<ContactRecord> _contactRepository;
        private readonly IRepository<EntityRecord> _entityRepository;
        private readonly IRepository<AddressRecord> _addressRepository;
        private readonly IRepository<ShowRecord> _showRepository;
        private readonly IRepository<EventMetadataRecord> _metadataRepository;

        public EntityService(
            IRepository<ContactRecord> contactRepository, 
            IRepository<EntityRecord> entityRepository,
            IRepository<AddressRecord> addressRepository, 
            IRepository<ShowRecord> showRepository,
            IRepository<EventMetadataRecord> metadataRepository)
        {
            _metadataRepository = metadataRepository;
            _showRepository = showRepository;
            _entityRepository = entityRepository;
            _contactRepository = contactRepository;
            _addressRepository = addressRepository;
        }

        #region Shows

        public void CheckShowVenue(int eventId, int venueId)
        {
            var shows =
                _showRepository
                    .Table
                    .Where(
                        s =>
                        s.EventMetadataRecord.Id == eventId &&
                        s.EntityRecord.Id == venueId &&
                        (!s.IsDeleted || s.IsReference))
                    .ToList();

            if (shows.Any())
            {
                // If we have both is reference and not is reference shows, remove all is reference shows
                if (shows.Count(s => !s.IsReference) > 0 && shows.Count(s => s.IsReference) > 0)
                {
                    foreach (var showRecord in shows.Where(s => s.IsReference))
                    {
                        _showRepository.Delete(showRecord);
                        _showRepository.Flush();
                    }
                }
            }
            else
            {
                var show = new ShowRecord
                    {
                        EventMetadataRecord = _metadataRepository.Get(eventId),
                        EntityRecord = _entityRepository.Get(venueId),
                        IsReference = true,
                        IsDeleted = false,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    };
                _showRepository.Create(show);
                _showRepository.Flush();
            }
        }

        public void SaveOrAddShow(ShowVm item, int eventMetadataId, int venueId)
        {
            var show = _showRepository.Get(item.Id);
            var metadata = _metadataRepository.Get(eventMetadataId);
            var venue = _entityRepository.Get(venueId);

            if (metadata == null || venue == null) return;

            if (show == null)
            {
                show = new ShowRecord
                    {
                        EventMetadataRecord = metadata,
                        EntityRecord = venue,
                        IsReference = false,
                        Title = item.Title,
                        Description = item.Description,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        Picture = item.Picture,
                        DetailsUrl = item.DetailsUrl,
                        IsDeleted = false,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    };

                _showRepository.Create(show);
                _showRepository.Flush();

                CheckShowVenue(metadata.Id, venue.Id);
            }
            else
            {
                show.EntityRecord = venue;
                show.Title = item.Title;
                show.Description = item.Description;
                show.StartTime = item.StartTime;
                show.EndTime = item.EndTime;
                show.Picture = item.Picture;
                show.DetailsUrl = item.DetailsUrl;
                show.DateModified = DateTime.UtcNow;

                _showRepository.Flush();
            }
        }

        #endregion

        #region Entities

        public bool IsNameExists(EntityVm item, EntityType type)
        {
            return _entityRepository.Table.Any(e => e.Type == (int)type && e.Id != item.Id && e.Name == item.Name);
        }

        public AccessType GetEntityAccess(SmartWalkUserRecord user, int entityId)
        {
            if (user == null)
                return AccessType.Deny;

            if (entityId == 0)
                return AccessType.AllowEdit;

            var entity = _entityRepository.Get(entityId);
            if (entity == null || entity.IsDeleted)
                return AccessType.Deny;

            return entity.SmartWalkUserRecord.Id == user.Id ? AccessType.AllowEdit : AccessType.AllowView;
        }

        public IList<EntityVm> GetEntities(
            SmartWalkUserRecord user,
            EntityType type,
            int pageNumber = 0,
            int pageSize = ViewSettings.ItemsLoad,
            Func<EntityRecord, IComparable> orderBy = null,
            bool isDesc = false,
            string searchString = null,
            int[] excludeIds = null)
        {
            return GetEntitiesInner(
                user == null ? (IEnumerable<EntityRecord>)_entityRepository.Table : user.Entities,
                type,
                pageNumber,
                pageSize,
                orderBy,
                isDesc,
                searchString,
                excludeIds);
        }

        private static IList<EntityVm> GetEntitiesInner(
            IEnumerable<EntityRecord> query,
            EntityType type,
            int pageNumber,
            int pageSize,
            Func<EntityRecord, IComparable> orderBy,
            bool isDesc,
            string searchString,
            int[] excludeIds = null)
        {
            query = query.Where(e => e.Type == (int)type && !e.IsDeleted);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(
                    e => e.Name.ToLower(CultureInfo.InvariantCulture).Contains(
                        searchString.ToLower(CultureInfo.InvariantCulture)));
            }

            if (excludeIds != null)
            {
                query = query.Where(
                    e => !excludeIds.Contains(e.Id));
            }

            if (orderBy != null)
            {
                query = isDesc ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }

            return
                query.Skip(pageSize*pageNumber)
                     .Take(pageSize)
                     .Select(e => ViewModelContractFactory.CreateViewModelContract(e, LoadMode.Compact))
                     .ToList();
        }

        public IList<EntityVm> GetAccesibleUserVenues(
            SmartWalkUserRecord user, 
            int eventId, 
            int pageNumber,
            int pageSize, 
            string searchString)
        {
            var metadata = user.EventMetadataRecords.FirstOrDefault(e => e.Id == eventId);

            var query =
                user.Entities.Where(
                    e =>
                    e.Type == (int)EntityType.Venue && !e.IsDeleted &&
                    (metadata == null || metadata.ShowRecords.All(s => s.EntityRecord.Id != e.Id || s.IsDeleted)));
            if (!string.IsNullOrEmpty(searchString))
                query =
                    query.Where(
                        e =>
                        e.Name.ToLower(CultureInfo.InvariantCulture)
                         .Contains(searchString.ToLower(CultureInfo.InvariantCulture)));

            return
                query.Skip(pageSize*pageNumber)
                     .Take(pageSize)
                     .Select(e => ViewModelContractFactory.CreateViewModelContract(e, LoadMode.Compact))
                     .ToList();
        }

        public EntityVm GetEntityVmById(int hostId, EntityType type)
        {
            var entity = _entityRepository.Get(hostId);

            return entity == null ? new EntityVm { Id = 0, Type = (int)type } : GetEntityVm(entity);
        }

        public EntityVm GetEntityVm(EntityRecord entity, LoadMode mode = LoadMode.Full)
        {
            return ViewModelContractFactory.CreateViewModelContract(entity, mode);
        }

        // TODO: To support event day
        public IList<EntityVm> GetEventEntities(EventMetadataRecord metadata, int? day = null)
        {
            return
                metadata.ShowRecords.Where(s => !s.IsDeleted)
                        .Select(s => s.EntityRecord)
                        .Distinct()
                        .Select(e => ViewModelContractFactory.CreateViewModelContract(e, metadata))
                        .ToList();
        }

        public EntityVm SaveOrAddEntity(SmartWalkUserRecord user, EntityVm entityVm)
        {
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
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow
                    };

                _entityRepository.Create(entity);
            }
            else
            {
                entity.Name = entityVm.Name;
                entity.Picture = entityVm.Picture;
                entity.Description = entityVm.Description;
                entity.DateModified = DateTime.Now;
            }

            _entityRepository.Flush();

            foreach (var contact in entityVm.Contacts)
            {
                if (contact.Destroy)
                    DeleteContact(contact.Id);
                else
                    entity.ContactRecords.Add(SaveOrAddContactInner(contact, entity.Id));
            }

            foreach (var address in entityVm.Addresses)
            {
                if (address.Destroy)
                    DeleteAddress(address.Id);
                else
                    entity.AddressRecords.Add(SaveOrAddAddressInner(address, entity.Id));
            }

            return GetEntityVm(entity);
        }

        public void DeleteEntity(int entityId)
        {
            var entity = _entityRepository.Get(entityId);

            if (entity == null)
                return;

            entity.IsDeleted = true;
            _entityRepository.Flush();
        }

        #endregion

        #region Addresses

        private AddressRecord SaveOrAddAddressInner(AddressVm addressVm, int entityId)
        {
            var address = _addressRepository.Get(addressVm.Id);

            if (address == null)
            {
                var entity = _entityRepository.Get(entityId);

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

        private void DeleteAddress(int addressId)
        {
            var address = _addressRepository.Get(addressId);

            if (address == null)
                return;

            _addressRepository.Delete(address);
            _addressRepository.Flush();
        }

        #endregion

        #region Contacts

        private ContactRecord SaveOrAddContactInner(ContactVm contactVm, int entityId)
        {
            var contact = _contactRepository.Get(contactVm.Id);

            if (contact == null)
            {
                var entity = _entityRepository.Get(entityId);

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
                contact.Type = contactVm.Type;
            }

            _contactRepository.Flush();

            return contact;
        }

        private void DeleteContact(int contactId)
        {
            var contact = _contactRepository.Get(contactId);

            if (contact == null)
                return;

            _contactRepository.Delete(contact);
            _contactRepository.Flush();
        }

        #endregion
    }

    public enum AccessType
    {
        Deny,
        AllowView,
        AllowEdit
    }
}
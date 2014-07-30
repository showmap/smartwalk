using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.Utils;
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

        public EntityService(
            IRepository<EntityRecord> entityRepository,
            IRepository<ContactRecord> contactRepository, 
            IRepository<AddressRecord> addressRepository)
        {
            _entityRepository = entityRepository;
            _contactRepository = contactRepository;
            _addressRepository = addressRepository;
        }

        public bool IsNameUnique(EntityVm item)
        {
            return !_entityRepository
                .Table
                .Any(e => 
                    e.Type == item.Type && 
                    e.Id != item.Id && 
                    e.Name == item.Name);
        }

        public AccessType GetEntityAccess(SmartWalkUserRecord user, int entityId)
        {
            if (user == null) return AccessType.Deny;

            var entity = _entityRepository.Get(entityId);
            if (entity == null || entity.IsDeleted) return AccessType.Deny;

            return entity.SmartWalkUserRecord.Id == user.Id 
                ? AccessType.AllowEdit 
                : AccessType.AllowView;
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
            var query = user == null 
                ? (IEnumerable<EntityRecord>)_entityRepository.Table 
                : user.Entities;

            var result = GetEntities(query, type, pageNumber, pageSize, 
                orderBy, isDesc, searchString, excludeIds);
            return result;
        }

        public EntityVm GetEntityById(int hostId)
        {
            var entity = _entityRepository.Get(hostId);
            if (entity == null) return null;

            var result = ViewModelContractFactory
                .CreateViewModelContract(entity, LoadMode.Full);
            return result;
        }

        public EntityVm SaveEntity(SmartWalkUserRecord user, EntityVm entityVm)
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
                {
                    DeleteContact(contact.Id);
                }
                else
                {
                    // TODO: Do we add a contact no matter if it's just updated?
                    entity.ContactRecords.Add(SaveContact(contact, entity.Id));
                }
            }

            foreach (var address in entityVm.Addresses)
            {
                if (address.Destroy)
                {
                    DeleteAddress(address.Id);
                }
                else
                {
                    // TODO: Do we add a address no matter if it's just updated?
                    entity.AddressRecords.Add(SaveAddress(address, entity.Id));
                }
            }

            var result = ViewModelContractFactory
                .CreateViewModelContract(entity, LoadMode.Full);
            return result;
        }

        public void DeleteEntity(int entityId)
        {
            var entity = _entityRepository.Get(entityId);
            if (entity == null) return;

            entity.IsDeleted = true;
            _entityRepository.Flush();
        }

        private static IList<EntityVm> GetEntities(
            IEnumerable<EntityRecord> query,
            EntityType type,
            int pageNumber,
            int pageSize,
            Func<EntityRecord, IComparable> orderBy,
            bool isDesc = false,
            string searchString = null,
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
                query = query.Where(e => !excludeIds.Contains(e.Id));
            }

            if (orderBy != null)
            {
                query = isDesc ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            }

            return
                query.Skip(pageSize * pageNumber)
                     .Take(pageSize)
                     .Select(e => ViewModelContractFactory.CreateViewModelContract(e, LoadMode.Compact))
                     .ToList();
        }

        private AddressRecord SaveAddress(AddressVm addressVm, int entityId)
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
            if (address == null) return;

            _addressRepository.Delete(address);
            _addressRepository.Flush();
        }

        private ContactRecord SaveContact(ContactVm contactVm, int entityId)
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
            if (contact == null) return;

            _contactRepository.Delete(contact);
            _contactRepository.Flush();
        }
    }
}
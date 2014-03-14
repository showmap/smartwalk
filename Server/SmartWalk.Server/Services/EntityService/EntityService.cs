using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EntityService
{
    public class EntityService : IEntityService
    {
        private readonly IRepository<ContactRecord> _contactRepository;
        private readonly IRepository<EntityRecord> _entityRepository;
        private readonly IRepository<AddressRecord> _addressRepository;

        public EntityService(IRepository<ContactRecord> contactRepository, IRepository<EntityRecord> entityRepository, IRepository<AddressRecord> addressRepository)
        {
            _entityRepository = entityRepository;
            _contactRepository = contactRepository;
            _addressRepository = addressRepository;
        }

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
            return _entityRepository.Table.Where(e => e.ShowRecords.Any(s => s.EventMetadataRecord == metadata)).Select(e => ViewModelContractFactory.CreateViewModelContract(e, metadata)).ToList();
        }

        public EntityRecord SaveOrAddEntity(SmartWalkUserRecord user, EntityVm entityVm) {
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
                    entity.ContactRecords.Add(SaveOrAddContact(entity, contact));
            }

            foreach (var address in entityVm.AllAddresses) {
                if (address.State == VmItemState.Deleted)
                    DeleteAddress(address.Id);
                else
                    entity.AddressRecords.Add(SaveOrAddAddress(entity, address));
            }

            return entity;
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


        public AddressRecord SaveOrAddAddress(EntityRecord entity, AddressVm addressVm)
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

        public ContactRecord SaveOrAddContact(EntityRecord host, ContactVm contactVm)
        {
            var contact = _contactRepository.Get(contactVm.Id);

            if (contact == null) {

                contact = new ContactRecord {
                    EntityRecord = host,
                    Type = contactVm.Type,
                    Title = contactVm.Title,
                    Contact = contactVm.Contact
                };

                _contactRepository.Create(contact);
            }
            else {
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
    }
}
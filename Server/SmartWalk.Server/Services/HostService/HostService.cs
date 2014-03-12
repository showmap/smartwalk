using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.HostService
{
    public class HostService : IHostService
    {
        private readonly IRepository<ContactRecord> _contactRepository;
        private readonly IRepository<EntityRecord> _entityRepository;

        public HostService(IRepository<ContactRecord> contactRepository, IRepository<EntityRecord> entityRepository) {
            _entityRepository = entityRepository;
            _contactRepository = contactRepository;
        }

        public IList<EntityVm> GetUserHosts(SmartWalkUserRecord user) {
            return user.Entities.Where(e => e.Type == (int) EntityType.Host).Select(ViewModelContractFactory.CreateViewModelContract).ToList();
        }

        public EntityVm GetHostVmById(int hostId) {
            var host = _entityRepository.Get(hostId);

            if (host == null || host.Type != (int) EntityType.Host)
                return new EntityVm {Id = 0};

            return ViewModelContractFactory.CreateViewModelContract(host);
        }

        public EntityRecord SaveOrAddHost(SmartWalkUserRecord user, EntityVm hostVm) {
            var host = _entityRepository.Get(hostVm.Id);

            if (host == null) {
                host = new EntityRecord {
                    Name = hostVm.Name,
                    Type = (int) EntityType.Host,
                    SmartWalkUserRecord = user,
                    Picture = hostVm.Picture,
                    Description = hostVm.Description,
                };

                _entityRepository.Create(host);
            }
            else {
                host.Picture = hostVm.Picture;
                host.Description = hostVm.Description;
            }
            
            _entityRepository.Flush();
            
            foreach (var contact in hostVm.AllContacts)
            {
                if (contact.State == ContactState.Deleted)
                    DeleteContact(contact.Id);
                else
                    host.ContactRecords.Add(SaveOrAddContact(host, contact));
            }

            return host;
        }

        public void DeleteHost(int hostId) {
            var host = _entityRepository.Get(hostId);

            if (host == null || host.Type != (int)EntityType.Host)
                return;

            foreach (var contact in host.ContactRecords) {
                _contactRepository.Delete(contact); 
                _contactRepository.Flush();
            }

            _entityRepository.Delete(host);
            _entityRepository.Flush();
        }


        public ContactRecord SaveOrAddContact(EntityRecord host, ContactVm contactVm) {
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
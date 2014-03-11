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
                return null;

            return ViewModelContractFactory.CreateViewModelContract(host);
        }

        public EntityRecord AddHost(SmartWalkUserRecord user, EntityVm hostVm)
        {
            var host = new EntityRecord
            {
                Name = hostVm.Name,
                Type = (int)EntityType.Host,
                SmartWalkUserRecord = user,
                Description = hostVm.Description,
                Picture = hostVm.Picture,
            };

            _entityRepository.Create(host);
            _entityRepository.Flush();

            foreach (var contact in hostVm.Contacts)
            {
                host.ContactRecords.Add(AddContact(host, contact));
            }

            return host;
        }

        public ContactRecord AddContact(EntityRecord host, ContactVm contactVm)
        {
            var contact = new ContactRecord
            {
                EntityRecord = host,
                Type = contactVm.Type,
                Title = contactVm.Title,
                Contact = contactVm.Contact
            };


            _contactRepository.Create(contact);
            _contactRepository.Flush();

            return contact;
        }
    }
}
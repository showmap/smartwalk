using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public class EventService : IEventService
    {
        private readonly IRepository<RegionRecord> _regionRepository;
        private readonly IRepository<EntityRecord> _entityRepository;
        private readonly IRepository<ContactRecord> _contactRepository;

        public EventService(IRepository<RegionRecord> regionRepository, IRepository<EntityRecord> entityRepository, IRepository<ContactRecord> contactRepository)
        {
            _regionRepository = regionRepository;
            _entityRepository = entityRepository;
            _contactRepository = contactRepository;
        }

        public IList<EventMetadataVm> GetUserEvents(SmartWalkUserRecord user) {
            return user.EventMetadataRecords.Select(ViewModelContractFactory.CreateViewModelContract).ToList();
        }

        public EventMetadataFullVm GetUserEventVmById(SmartWalkUserRecord user, int id) {
            return new EventMetadataFullVm {
                EventMetadata = ViewModelContractFactory.CreateViewModelContract(user.EventMetadataRecords.FirstOrDefault(u => u.Id == id)),
                Hosts = user.Entities.Where(e => e.Type == (int)EntityType.Host).Select(ViewModelContractFactory.CreateViewModelContract).ToList(),
                Regions = _regionRepository.Table.Select(ViewModelContractFactory.CreateViewModelContract).ToList()
            };
        }

        public EntityVm GetEntityVmById(int entityId) {
            return ViewModelContractFactory.CreateViewModelContract(_entityRepository.Get(entityId));
        }

        public void AddHost(SmartWalkUserRecord user, EntityVm hostVm) {
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

            foreach (var contact in hostVm.Contacts) {
                host.ContactRecords.Add(AddContact(host, contact));
            }
        }

        public ContactRecord AddContact(EntityRecord host, ContactVm contactVm) {
            var contact = new ContactRecord {
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
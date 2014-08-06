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
        private readonly IRepository<EntityRecord> _entityRepository;

        public EntityService(IRepository<EntityRecord> entityRepository)
        {
            _entityRepository = entityRepository;
        }

        public bool IsNameUnique(EntityVm entityVm)
        {
            return !_entityRepository
                .Table
                .Any(e => 
                    e.Type == entityVm.Type && 
                    e.Id != entityVm.Id && 
                    e.Name == entityVm.Name);
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

        public EntityVm GetEntityById(int entityId)
        {
            var entity = _entityRepository.Get(entityId);
            if (entity == null) return null;

            var result = ViewModelFactory.CreateViewModel(entity, LoadMode.Full);
            return result;
        }

        public EntityVm SaveEntity(SmartWalkUserRecord user, EntityVm entityVm)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (entityVm == null) throw new ArgumentNullException("entityVm");

            var entity = _entityRepository.Get(entityVm.Id) ?? new EntityRecord
                {
                    SmartWalkUserRecord = user,
                    DateCreated = DateTime.UtcNow
                };

            ViewModelFactory.UpdateByViewModel(entity, entityVm);

            foreach (var contactVm in entityVm.Contacts)
            {
                // ReSharper disable AccessToForEachVariableInClosure
                var contact = entity.ContactRecords.FirstOrDefault(cr => cr.Id == contactVm.Id);
                // ReSharper restore AccessToForEachVariableInClosure
                if (contact == null)
                {
                    contact = new ContactRecord { EntityRecord = entity };
                    entity.ContactRecords.Add(contact);
                }

                ViewModelFactory.UpdateByViewModel(contact, contactVm);
            }

            foreach (var addressVm in entityVm.Addresses)
            {
                // ReSharper disable AccessToForEachVariableInClosure
                var address = entity.AddressRecords.FirstOrDefault(ar => ar.Id == addressVm.Id);
                // ReSharper restore AccessToForEachVariableInClosure
                if (address == null)
                {
                    address = new AddressRecord { EntityRecord = entity };
                    entity.AddressRecords.Add(address);
                }

                ViewModelFactory.UpdateByViewModel(address, addressVm);
            }

            if (entity.Id == 0)
            {
                _entityRepository.Create(entity);
            }

            _entityRepository.Flush();

            var result = ViewModelFactory.CreateViewModel(entity, LoadMode.Full);
            return result;
        }

        public void DeleteEntity(int entityId)
        {
            var entity = _entityRepository.Get(entityId);
            if (entity == null) return;

            foreach (var contact in entity.ContactRecords.Where(c => !c.IsDeleted).ToArray())
            {
                contact.IsDeleted = true;
            }

            foreach (var address in entity.AddressRecords.Where(a => !a.IsDeleted).ToArray())
            {
                address.IsDeleted = true;
            }

            entity.IsDeleted = true;
            entity.DateModified = DateTime.UtcNow;

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
                        // ReSharper disable PossibleNullReferenceException
                        searchString.ToLower(CultureInfo.InvariantCulture)));
                        // ReSharper restore PossibleNullReferenceException
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
                     .Select(e => ViewModelFactory.CreateViewModel(e, LoadMode.Compact))
                     .ToArray();
        }
    }
}
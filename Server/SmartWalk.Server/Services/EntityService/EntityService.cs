using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Orchard;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.Base;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Shared;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.EntityService
{
    [UsedImplicitly]
    public class EntityService : OrchardBaseService, IEntityService
    {
        private readonly IRepository<EntityRecord> _entityRepository;

        public EntityService(
            IOrchardServices orchardServices,
            IRepository<EntityRecord> entityRepository)
            : base(orchardServices)
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
                    e.Name == entityVm.Name &&
                    !e.IsDeleted);
        }

        public bool IsDeletable(int entityId)
        {
            var entity = _entityRepository.Get(entityId);
            var result = IsDeletable(entity);
            return result;
        }

        public AccessType GetEntitiesAccess()
        {
            var result = SecurityUtils.GetAccess(Services.Authorizer);
            return result;
        }

        public AccessType GetEntityAccess(int entityId)
        {
            var entity = _entityRepository.Get(entityId);
            var result = entity.GetAccess(Services.Authorizer, CurrentUserRecord);
            return result;
        }

        public IList<EntityVm> GetEntities(DisplayType display, EntityType type,
            int pageNumber, int pageSize, bool isDesc, 
            string searchString, int[] excludeIds)
        {
            var access = GetEntitiesAccess();
            if (access == AccessType.Deny)
                throw new SecurityException("Can't get entites.");

            if (display.Has(DisplayType.My) && CurrentUserRecord == null) 
                throw new SecurityException("Can't show my entities without user.");

            IQueryable<EntityRecord> query;

            if (display.Has(DisplayType.All) && display.Has(DisplayType.My))
            {
                query = _entityRepository.Table.WherePublicAndMine(type, CurrentUserRecord.Id);
            }
            else if (display.Has(DisplayType.My))
            {
                query = CurrentUserRecord.Entities.AsQueryable();
            }
            else if (display.Has(DisplayType.All))
            {
                query = _entityRepository.Table.WherePublic(type);
            }
            else
            {
                return null;
            }

            var result = GetEntities(query, type, pageNumber, pageSize, 
                isDesc, searchString, excludeIds);
            return result;
        }

        public EntityVm GetEntityById(int entityId)
        {
            var entity = _entityRepository.Get(entityId);
            if (entity == null || entity.IsDeleted) return null;

            var access = entity.GetAccess(Services.Authorizer, CurrentUserRecord);
            if (access == AccessType.Deny)
                throw new SecurityException("Can't get entity.");

            var result = ViewModelFactory.CreateViewModel(entity, LoadMode.Full);
            return result;
        }

        public EntityVm SaveEntity(EntityVm entityVm)
        {
            if (entityVm == null) throw new ArgumentNullException("entityVm");

            var entity = _entityRepository.Get(entityVm.Id) ?? new EntityRecord
                {
                    SmartWalkUserRecord = CurrentUserRecord,
                    DateCreated = DateTime.UtcNow
                };

            var access = entity.GetAccess(Services.Authorizer, CurrentUserRecord);
            if (access != AccessType.AllowEdit) 
                throw new SecurityException("Can't edit entity.");

            if (entity.IsDeleted)
                throw new InvalidOperationException("Can't edit deleted entity.");

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
            if (entity == null || entity.IsDeleted) return;

            var access = entity.GetAccess(Services.Authorizer, CurrentUserRecord);
            if (access != AccessType.AllowEdit)
                throw new SecurityException("Can't delete entity.");

            if (!IsDeletable(entity)) 
                throw new InvalidOperationException("Can't delete entity that has references from events.");

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
            IQueryable<EntityRecord> query,
            EntityType type,
            int pageNumber,
            int pageSize,
            bool isDesc = false,
            string searchString = null,
            int[] excludeIds = null)
        {
            query = query.Where(e => e.Type == (byte)type && !e.IsDeleted);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query
                    .Where(e => e.Name != null
                                && e.Name.ToUpperInvariant().Contains(searchString.ToUpperInvariant()));
            }

            if (excludeIds != null)
            {
                query = query.Where(e => !excludeIds.Contains(e.Id));
            }

            query =
                isDesc
                    ? query.OrderByDescending(e => e.Name)
                    : query.OrderBy(e => e.Name);

            return
                query.Skip(pageSize * pageNumber)
                     .Take(pageSize)
                     .Select(e => ViewModelFactory.CreateViewModel(e, LoadMode.Compact))
                     .ToArray();
        }

        private static bool IsDeletable(EntityRecord entity)
        {
            if (entity == null || entity.IsDeleted) return false;

            var result =
                entity.EventMetadataRecords.All(em => em.IsDeleted)
                && entity.ShowRecords.All(s => s.IsDeleted);
            return result;
        }
    }

    // TODO: The intentional copy-paste should be eliminated, if there is a solution
    // The Where expression should be passed in the way that it's one SQL query
    // if intermediate functions are used the nHibernate create dozen of queries
    public static class EntityServiceExtensions
    {
        /// <summary>
        /// Returns the subset of entities that have been used in any of public events.
        /// </summary>
        public static IQueryable<EntityRecord> WherePublic(
            this IQueryable<EntityRecord> table, EntityType entityType)
        {
            if (entityType == EntityType.Host)
            {
                return table.Where(
                    ent => ent.EventMetadataRecords
                              .Any(emr => !emr.IsDeleted
                                          && emr.Status == (byte)EventStatus.Public));
            }

            if (entityType == EntityType.Venue)
            {
                return table.Where(
                    ent => ent.ShowRecords
                              .Any(sh => !sh.IsDeleted
                                         && !sh.EventMetadataRecord.IsDeleted
                                         && sh.EventMetadataRecord.Status == (byte)EventStatus.Public));
            }

            return table;
        }

        /// <summary>
        /// Returns the subset of entities that have been used in any of public events and current user's ones.
        /// </summary>
        public static IQueryable<EntityRecord> WherePublicAndMine(
            this IQueryable<EntityRecord> table, EntityType entityType, int currentUserId)
        {
            if (entityType == EntityType.Host)
            {
                return table
                    .Where(ent =>
                           ent.SmartWalkUserRecord.Id == currentUserId
                           || ent.EventMetadataRecords
                                 .Any(emr => !emr.IsDeleted
                                             && emr.Status == (byte)EventStatus.Public));
            }

            if (entityType == EntityType.Venue)
            {
                return table
                    .Where(ent =>
                           ent.SmartWalkUserRecord.Id == currentUserId
                           || ent.ShowRecords
                                 .Any(sh => !sh.IsDeleted
                                            && !sh.EventMetadataRecord.IsDeleted
                                            && sh.EventMetadataRecord.Status == (byte)EventStatus.Public));
            }

            return table;
        }
    }
}
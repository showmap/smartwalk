﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using Orchard;
using Orchard.Data;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.Base;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;
using SmartWalk.Shared;

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

        public AccessType GetEntitiesAccess()
        {
            var result = SecurityUtils.GetAccess(Services.Authorizer);
            return result;
        }

        public AccessType GetEntityAccess(int entityId)
        {
            var entity = _entityRepository.Get(entityId);
            var result = entity.GetAccess(Services.Authorizer, CurrentUser);
            return result;
        }

        public IList<EntityVm> GetEntities(
            DisplayType display,
            EntityType type,
            int pageNumber = 0,
            int pageSize = ViewSettings.ItemsLoad,
            Func<EntityRecord, IComparable> orderBy = null,
            bool isDesc = false,
            string searchString = null,
            int[] excludeIds = null)
        {
            var access = GetEntitiesAccess();
            if (access == AccessType.Deny)
                throw new SecurityException("Can't get entites.");

            if (display == DisplayType.My && CurrentUser == null) 
                throw new SecurityException("Can't show my entities without user.");

            // admin users treat all enetities as theirs
            var query = display == DisplayType.All
                ? (IEnumerable<EntityRecord>)_entityRepository.Table 
                : CurrentUser.Entities;

            var result = GetEntities(query, type, pageNumber, pageSize, 
                orderBy, isDesc, searchString, excludeIds);
            return result;
        }

        public EntityVm GetEntityById(int entityId)
        {
            var entity = _entityRepository.Get(entityId);
            if (entity == null || entity.IsDeleted) return null;

            var access = entity.GetAccess(Services.Authorizer, CurrentUser);
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
                    SmartWalkUserRecord = CurrentUser,
                    DateCreated = DateTime.UtcNow
                };

            var access = entity.GetAccess(Services.Authorizer, CurrentUser);
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

            var access = entity.GetAccess(Services.Authorizer, CurrentUser);
            if (access != AccessType.AllowEdit)
                throw new SecurityException("Can't delete entity.");

            if (!entity.IsDeletable()) 
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
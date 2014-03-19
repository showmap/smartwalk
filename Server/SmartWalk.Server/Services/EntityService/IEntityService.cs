using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EntityService
{
    public interface IEntityService : IDependency {
        IList<EntityVm> GetUserEntities(SmartWalkUserRecord user, EntityType type);
        IList<EntityVm> GetEventEntities(EventMetadataRecord eventRecord);
        EntityVm GetEntityVmById(int entityId, EntityType type);
        EntityVm GetEntityVm(EntityRecord entity);
        EntityVm SaveOrAddEntity(SmartWalkUserRecord user, EntityVm entityVm);
        void DeleteEntity(int hostId);

        AddressVm SaveOrAddAddress(EntityRecord entity, AddressVm addressVm);
        void DeleteAddress(int addressId);
        ContactVm SaveOrAddContact(EntityRecord entity, ContactVm contactVm);
        void DeleteContact(int contactId);

        ShowVm SaveOrAddShow(ShowVm item);
    }
}
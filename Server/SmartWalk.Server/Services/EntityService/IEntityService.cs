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
        EntityVm GetEntityVmById(int entityId, EntityType type);
        EntityRecord SaveOrAddEntity(SmartWalkUserRecord user, EntityVm entityVm);
        void DeleteEntity(int hostId);

        AddressRecord SaveOrAddAddress(EntityRecord entity, AddressVm addressVm);
        void DeleteAddress(int addressId);
        ContactRecord SaveOrAddContact(EntityRecord entity, ContactVm contactVm);
        void DeleteContact(int contactId);
    }
}
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
        IList<EntityVm> GetUserEntities(SmartWalkUserRecord user, EntityType type, int pageNumber, int pageSize, Func<EntityRecord, bool> where, Func<EntityRecord, IComparable> orderBy, bool isDesc);
        IList<EntityVm> GetEventEntities(EventMetadataRecord eventRecord);
        IList<EntityVm> GetAccesibleUserVenues(SmartWalkUserRecord user, int eventId, int pageNumber, int pageSize, Func<EntityRecord, bool> where);
        EntityVm GetEntityVmById(int entityId, EntityType type);
        EntityVm GetEntityVm(EntityRecord entity);
        EntityVm SaveOrAddEntity(SmartWalkUserRecord user, EntityVm entityVm);
        void DeleteEntity(int hostId);

        AddressVm SaveOrAddAddress(AddressVm addressVm);
        void DeleteAddress(int addressId);
        AddressVm GetAddress(int addressId);

        ContactVm SaveOrAddContact(ContactVm contactVm);
        void DeleteContact(int contactId);
        ContactVm GetContact(int contactId);

        ShowVm SaveOrAddShow(ShowVm item);
        void DeleteShow(int showId);
        void DeleteEventVenue(EntityVm item);
        ShowVm SaveEventVenue(EntityVm item);
        ShowVm GetShow(int showId);
    }
}
using System;
using System.Collections.Generic;
using Orchard;
using SmartWalk.Server.Records;
using SmartWalk.Server.Services.EventService;
using SmartWalk.Server.ViewModels;
using SmartWalk.Server.Views;

namespace SmartWalk.Server.Services.EntityService
{
    public interface IEntityService : IDependency
    {
        IList<EntityVm> GetEntities(
            SmartWalkUserRecord user,
            EntityType type,
            int pageNumber = 0,
            int pageSize = ViewSettings.ItemsLoad,
            Func<EntityRecord, IComparable> orderBy = null,
            bool isDesc = false,
            string searchString = null,
            int[] excludeIds = null);

        IList<EntityVm> GetEventEntities(EventMetadataRecord eventRecord);
        bool IsNameExists(EntityVm item, EntityType type);
        AccessType GetEntityAccess(SmartWalkUserRecord user, int entityId);
        EntityVm GetEntityVmById(int entityId, EntityType type);
        EntityVm GetEntityVm(EntityRecord entity, LoadMode mode = LoadMode.Full);
        EntityVm SaveOrAddEntity(SmartWalkUserRecord user, EntityVm entityVm);
        void DeleteEntity(int hostId);

        ShowVm SaveOrAddShow(ShowVm item, int eventMetadataId, int venueId);
        ShowVm CheckShowVenue(int eventId, int venueId);
    }
}
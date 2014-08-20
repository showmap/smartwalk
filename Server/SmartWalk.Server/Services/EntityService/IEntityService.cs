using System.Collections.Generic;
using Orchard;
using SmartWalk.Server.Records;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EntityService
{
    public interface IEntityService : IDependency
    {
        bool IsNameUnique(EntityVm entityVm);
        bool IsDeletable(int entityId);

        AccessType GetEntitiesAccess();
        AccessType GetEntityAccess(int entityId);

        IList<EntityVm> GetEntities(DisplayType display, EntityType type,
            int pageNumber, int pageSize, bool isDesc = false, 
            string searchString = null, int[] excludeIds = null);
        EntityVm GetEntityById(int entityId);
        EntityVm SaveEntity(EntityVm entityVm);
        void DeleteEntity(int hostId);
    }
}
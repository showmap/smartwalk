using System;
using System.Collections.Generic;
using Orchard;
using SmartWalk.Server.Records;
using SmartWalk.Server.Utils;
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

        bool IsNameUnique(EntityVm entityVm);
        AccessType GetEntityAccess(SmartWalkUserRecord user, int entityId);
        EntityVm GetEntityById(int entityId);

        EntityVm SaveEntity(SmartWalkUserRecord user, EntityVm entityVm);
        void DeleteEntity(int hostId);
    }
}
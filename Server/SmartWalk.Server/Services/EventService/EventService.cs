using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public class EventService : IEventService
    {
        public IList<EventMetadataVm> GetUserEvents(SmartWalkUserRecord user) {
            return user.EventMetadataRecords.Select(u => new EventMetadataVm {
                Id = u.Id,
                HostId = u.HostRecord.Id,
                HostName = u.HostRecord.Name,
                Title = u.Title,
                CombineType = u.CombineType,
                StartTime = u.StartTime.ToString("d",  CultureInfo.InvariantCulture),
                EndTime = u.EndTime.HasValue ? u.EndTime.Value.ToString("d",  CultureInfo.InvariantCulture) : "",
                IsMobileReady = u.IsMobileReady,
                IsWidgetReady = u.IsWidgetReady,
                Description = u.Description,
                DateCreated = u.DateCreated.ToString("d",  CultureInfo.InvariantCulture),
                DateModified = u.DateModified.ToString("d", CultureInfo.InvariantCulture),
            }).ToList();
        }
    }
}
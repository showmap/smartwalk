using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public static class ViewModelContractFactory
    {               
        public static EventMetadataVm CreateViewModelContract(EventMetadataRecord record) {
            if (record == null)
                return null;

            return new EventMetadataVm
            {
                Id = record.Id,
                UserId = record.SmartWalkUserRecord.Id,
                HostId = record.EntityRecord.Id,
                HostName = record.EntityRecord.Name,
                Title = record.Title,
                CombineType = record.CombineType,
                StartTime = record.StartTime.ToString("d", CultureInfo.InvariantCulture),
                EndTime = record.EndTime.HasValue ? record.EndTime.Value.ToString("d", CultureInfo.InvariantCulture) : "",
                Latitude = record.Latitude,
                Longitude = record.Longitude,
                IsPublic = record.IsPublic,
                Description = record.Description,
                Picture = record.Picture,
                DateCreated = record.DateCreated.ToString("d", CultureInfo.InvariantCulture),
                DateModified = record.DateModified.ToString("d", CultureInfo.InvariantCulture),                
            };
        }
    }
}
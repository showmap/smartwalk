using System;
using SmartWalk.Server.Records;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public static class ViewModelFactory
    {
        public static EventMetadataVm CreateViewModel(EventMetadataRecord record, LoadMode mode)
        {
            if (record == null) throw new ArgumentNullException("record");

            var result = new EventMetadataVm { Id = record.Id };

            if (mode == LoadMode.Compact || mode == LoadMode.Full)
            {
                result.Title = record.Title;
                result.StartDate = record.StartTime;
                result.EndDate = record.EndTime;
                result.Picture = record.Picture;
            }

            if (mode == LoadMode.Full)
            {
                result.CombineType = record.CombineType;
                result.Latitude = record.Latitude;
                result.Longitude = record.Longitude;
                result.IsPublic = record.IsPublic;
                result.Description = record.Description;
            }

            return result;
        }

        public static ShowVm CreateViewModel(ShowRecord record)
        {
            if (record == null) throw new ArgumentNullException("record");

            return new ShowVm
                {
                    Id = record.Id,
                    Title = record.Title,
                    Description = record.Description,
                    StartTime = record.StartTime,
                    EndTime = record.EndTime,
                    Picture = record.Picture,
                    DetailsUrl = record.DetailsUrl
                };
        }

        public static void UpdateByViewModel(EventMetadataRecord record, EventMetadataVm eventVm, EntityRecord host)
        {
            if (!eventVm.StartDate.HasValue) throw new ArgumentNullException("eventVm.StartDate");

            record.DateModified = DateTime.UtcNow;

            record.EntityRecord = host;
            record.Title = eventVm.Title;
            record.Picture = eventVm.Picture;
            record.Description = eventVm.Description;
            record.StartTime = eventVm.StartDate.Value;
            record.EndTime = eventVm.EndDate;
            record.CombineType = eventVm.CombineType;
            record.IsPublic = eventVm.IsPublic;
        }

        public static void UpdateByViewModel(ShowRecord record, ShowVm showVm)
        {
            record.DateModified = DateTime.UtcNow;

            record.Title = showVm.Title;
            record.Description = showVm.Description;
            record.StartTime = showVm.StartTime;
            record.EndTime = showVm.EndTime;
            record.Picture = showVm.Picture;
            record.DetailsUrl = showVm.DetailsUrl;

            record.IsDeleted = showVm.Destroy;
        }
    }
}
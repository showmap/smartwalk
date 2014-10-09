using System;
using SmartWalk.Server.Records;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Shared.Utils;

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
                result.CombineType = (CombineType)record.CombineType;
                result.VenueOrderType = (VenueOrderType)record.VenueOrderType;
                result.Latitude = record.Latitude;
                result.Longitude = record.Longitude;
                result.Status = (EventStatus)record.Status;
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
            record.Title = eventVm.Title.StripTags();
            record.Picture = eventVm.Picture.StripTags();
            record.Description = eventVm.Description.StripTags();
            record.StartTime = eventVm.StartDate.Value;
            record.EndTime = eventVm.EndDate;
            record.CombineType = (byte)eventVm.CombineType;
            record.Status = (byte)eventVm.Status;
            record.VenueOrderType = (byte)eventVm.VenueOrderType;
        }

        public static void UpdateByViewModel(ShowRecord record, ShowVm showVm)
        {
            record.DateModified = DateTime.UtcNow;

            record.Title = showVm.Title.StripTags();
            record.Description = showVm.Description.StripTags();
            record.StartTime = showVm.StartTime;
            record.EndTime = showVm.EndTime;
            record.Picture = showVm.Picture.StripTags();
            record.DetailsUrl = showVm.DetailsUrl.StripTags();

            record.IsDeleted = showVm.Destroy;
        }
    }
}
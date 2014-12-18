using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.FileSystems.Media;
using SmartWalk.Server.Records;
using SmartWalk.Server.Utils;
using SmartWalk.Server.ViewModels;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Services.EventService
{
    public static class ViewModelFactory
    {
        public static EventMetadataVm CreateViewModel(EventMetadataRecord record, LoadMode mode,
            IStorageProvider storageProvider)
        {
            if (record == null) throw new ArgumentNullException("record");

            var result = new EventMetadataVm { Id = record.Id };

            if (mode == LoadMode.Compact || mode == LoadMode.Full)
            {
                result.Title = record.Title;
                result.StartDate = record.StartTime;
                result.EndDate = record.EndTime;
                result.Picture = FileUtil.GetPictureUrl(record.Picture, storageProvider);
            }

            if (mode == LoadMode.Full)
            {
                result.CombineType = (CombineType)record.CombineType;
                result.VenueOrderType = (VenueOrderType)record.VenueOrderType;
                result.VenueTitleFormatType = (VenueTitleFormatType)record.VenueTitleFormatType;
                result.Latitude = record.Latitude;
                result.Longitude = record.Longitude;
                result.Status = (EventStatus)record.Status;
                result.Description = record.Description;
            }

            return result;
        }

        public static ShowVm CreateViewModel(ShowRecord record, IStorageProvider storageProvider)
        {
            if (record == null) throw new ArgumentNullException("record");

            return new ShowVm
                {
                    Id = record.Id,
                    Title = record.Title,
                    Description = record.Description,
                    StartTime = record.StartTime,
                    EndTime = record.EndTime,
                    Picture = FileUtil.GetPictureUrl(record.Picture, storageProvider),
                    DetailsUrl = record.DetailsUrl
                };
        }

        public static void UpdateByViewModel(EventMetadataRecord record, EventMetadataVm eventVm, EntityRecord host)
        {
            if (!eventVm.StartDate.HasValue) throw new ArgumentNullException("eventVm.StartDate");

            record.DateModified = DateTime.UtcNow;

            record.EntityRecord = host;
            record.Title = eventVm.Title.TrimIt().StripTags();
            record.Description = eventVm.Description.TrimIt().StripTags();
            record.StartTime = eventVm.StartDate.Value;
            record.EndTime = eventVm.EndDate;
            record.CombineType = (byte)eventVm.CombineType;
            record.Status = (byte)eventVm.Status;
            record.VenueOrderType = (byte)eventVm.VenueOrderType;
            record.VenueTitleFormatType = (byte)eventVm.VenueTitleFormatType;
        }

        public static void UpdateByViewModel(ShowRecord record, ShowVm showVm)
        {
            record.DateModified = DateTime.UtcNow;

            record.Title = showVm.Title.TrimIt().StripTags();
            record.Description = showVm.Description.TrimIt().StripTags();
            record.StartTime = showVm.StartTime;
            record.EndTime = showVm.EndTime;
            record.DetailsUrl = showVm.DetailsUrl.TrimIt().StripTags();

            record.IsDeleted = showVm.Destroy;
        }

        public static void UpdateByViewModelPicture(EventMetadataRecord record, EventMetadataVm eventVm,
            Dictionary<ShowRecord, ShowVm> shows, IStorageProvider storageProvider)
        {
            var previousPictureUrl = FileUtil.GetPictureUrl(record.Picture, storageProvider);
            if (previousPictureUrl != eventVm.Picture)
            {
                record.Picture = FileUtil.ProcessUploadedPicture(record.Picture, eventVm.Picture,
                    string.Format("event/{0}", record.Id), storageProvider);
            }

            foreach (var showRec in record.ShowRecords
                .Where(s => !s.IsReference && !s.IsDeleted)
                .ToArray())
            {
                ShowVm showVm;
                if (shows.TryGetValue(showRec, out showVm))
                {
                    UpdateByViewModelPicture(record, showRec, showVm, storageProvider);
                }
            }
        }

        private static void UpdateByViewModelPicture(EventMetadataRecord eventRec, ShowRecord record, 
            ShowVm showVm, IStorageProvider storageProvider)
        {
            var previousPictureUrl = FileUtil.GetPictureUrl(record.Picture, storageProvider);
            if (previousPictureUrl != showVm.Picture)
            {
                record.Picture = FileUtil.ProcessUploadedPicture(record.Picture, showVm.Picture,
                    string.Format("event/{0}/shows", eventRec.Id), storageProvider);
            }
        }
    }
}
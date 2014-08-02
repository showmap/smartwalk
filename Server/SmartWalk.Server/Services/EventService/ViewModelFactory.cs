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

            switch (mode)
            {
                case LoadMode.Compact:
                    return new EventMetadataVm
                        {
                            Id = record.Id,
                            Title = record.Title,
                            StartDate = record.StartTime,
                            EndDate = record.EndTime,
                            Picture = record.Picture
                        };

                case LoadMode.Full:
                    return new EventMetadataVm
                        {
                            Id = record.Id,
                            Title = record.Title,
                            CombineType = record.CombineType,
                            StartDate = record.StartTime,
                            EndDate = record.EndTime,
                            Latitude = record.Latitude,
                            Longitude = record.Longitude,
                            IsPublic = record.IsPublic,
                            Description = record.Description,
                            Picture = record.Picture
                        };

                default:
                    return null;
            }
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
    }
}
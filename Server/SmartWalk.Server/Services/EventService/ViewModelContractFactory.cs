using System.Globalization;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public static class ViewModelContractFactory
    {
        public static EventMetadataVm CreateViewModelContract(EventMetadataRecord record, LoadMode mode)
        {
            if (record == null)
                return null;

            switch (mode)
            {
                case LoadMode.Compact:
                    return new EventMetadataVm
                        {
                            Id = record.Id,
                            Title = string.IsNullOrEmpty(record.Title) ? record.EntityRecord.Name : record.Title,
                            // TODO: Just pass timestamp, no?
                            StartTime = record.StartTime.ToString("d", CultureInfo.InvariantCulture),
                            EndTime =
                                record.EndTime.HasValue
                                    ? record.EndTime.Value.ToString("d", CultureInfo.InvariantCulture)
                                    : null,
                            Picture = record.Picture,
                            // TODO: possible should be dropped
                            DisplayDate = record.StartTime.ToString("D", CultureInfo.InvariantCulture)
                        };

                case LoadMode.Full:
                    return new EventMetadataVm
                        {
                            Id = record.Id,
                            Title = record.Title,
                            CombineType = record.CombineType,
                            // TODO: Just pass timestamp, no?
                            StartTime = record.StartTime.ToString("d", CultureInfo.InvariantCulture),
                            EndTime =
                                record.EndTime.HasValue
                                    ? record.EndTime.Value.ToString("d", CultureInfo.InvariantCulture)
                                    : null,
                            Latitude = record.Latitude,
                            Longitude = record.Longitude,
                            IsPublic = record.IsPublic,
                            Description = record.Description,
                            Picture = record.Picture,
                            // TODO: possible should be dropped
                            DisplayDate = record.StartTime.ToString("D", CultureInfo.InvariantCulture)
                        };

                default:
                    return null;
            }
        }
    }
}
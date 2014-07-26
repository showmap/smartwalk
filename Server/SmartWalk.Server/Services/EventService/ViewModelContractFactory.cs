using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public static class ViewModelContractFactory
    {
        public static EventMetadataVm CreateViewModelContract(EventMetadataRecord record, LoadMode mode)
        {
            if (record == null) return null;

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
    }
}
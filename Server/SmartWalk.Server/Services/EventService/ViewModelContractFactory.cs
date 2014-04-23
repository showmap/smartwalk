using System.Globalization;
using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;

namespace SmartWalk.Server.Services.EventService
{
    public static class ViewModelContractFactory
    {               
        public static EventMetadataVm CreateViewModelContract(EventMetadataRecord record, LoadMode mode) {
            if (record == null)
                return null;

            switch (mode) {
                case LoadMode.Compact:
                    return new EventMetadataVm
                    {
                        Id = record.Id,
                        UserId = record.SmartWalkUserRecord.Id,
                        Title = record.Title,
                        StartTime = record.StartTime.ToString("d", CultureInfo.InvariantCulture),
                        EndTime = record.EndTime.HasValue ? record.EndTime.Value.ToString("d", CultureInfo.InvariantCulture) : "",
                        IsPublic = record.IsPublic,
                        Picture = record.Picture
                    };

                case LoadMode.Full:
                default:
                    return new EventMetadataVm
                    {
                        Id = record.Id,
                        UserId = record.SmartWalkUserRecord.Id,
                        Title = record.Title,
                        CombineType = record.CombineType,
                        StartTime = record.StartTime.ToString("d", CultureInfo.InvariantCulture),
                        EndTime = record.EndTime.HasValue ? record.EndTime.Value.ToString("d", CultureInfo.InvariantCulture) : "",
                        Latitude = record.Latitude,
                        Longitude = record.Longitude,
                        IsPublic = record.IsPublic,
                        Description = record.Description,
                        Picture = record.Picture,
                        DisplayDate = record.StartTime.ToString("D", CultureInfo.InvariantCulture)
                    };
            }            
        }
    }
}
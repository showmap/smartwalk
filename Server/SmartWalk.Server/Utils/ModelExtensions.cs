using SmartWalk.Server.Records;
using SmartWalk.Server.ViewModels;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.Utils
{
    public static class ModelExtensions
    {
        public static bool IsMultiDay(this EventMetadataVm eventMetadata)
        {
            return DateTimeExtensions.IsMultiDay(eventMetadata.StartDate, eventMetadata.EndDate);
        }

        public static bool IsMultiDay(this EventMetadataRecord eventMetadata)
        {
            return DateTimeExtensions.IsMultiDay(eventMetadata.StartTime, eventMetadata.EndTime);
        }
    }
}
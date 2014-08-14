using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orchard.Localization;
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

        // TODO: To move this to a editing metadata
        public static bool IsDeletable(this EntityRecord entity)
        {
            var result = entity.EventMetadataRecords.All(em => em.IsDeleted) &&
                         entity.ShowRecords.All(s => s.IsDeleted);
            return result;
        }

        public static string DisplayDate(this EventMetadataVm eventMeta, CultureInfo culture)
        {
            return eventMeta.StartDate.ToString("D", culture) +
                    (eventMeta.EndDate.HasValue ? " - " + eventMeta.EndDate.ToString("D", culture) : string.Empty);
        }

        public static IList<AddressVm> Addresses(this EventMetadataVm eventMeta)
        {
            return eventMeta.Venues == null ? null : eventMeta.Venues.SelectMany(v => v.Addresses).ToList();
        }

        public static string DisplayName(this EventMetadataVm eventMeta)
        {
            return eventMeta.Title ?? (eventMeta.Host != null ? eventMeta.Host.Name : null);
        }

        public static string DisplayPicture(this EventMetadataVm eventMeta)
        {
            return eventMeta.Picture ?? (eventMeta.Host != null ? eventMeta.Host.Picture : null);
        }

        public static string DisplayTime(this ShowVm show, CultureInfo culture, Localizer localizer)
        {
            var result = show.StartTime.ToString("t", culture) +
                   (show.EndTime.HasValue ? " - " + show.EndTime.ToString("t", culture) : string.Empty);

            if (string.IsNullOrEmpty(result))
            {
                result = localizer("All day").Text;
            }

            return result;
        }

        public static bool HasAddresses(this EntityVm entity)
        {
            return entity.Addresses.Any(ad => ad.Address != null);
        }

        public static AddressVm FirstAddress(this EntityVm entity)
        {
            return entity.Addresses.FirstOrDefault(ad => ad.Address != null);
        }
    }
}
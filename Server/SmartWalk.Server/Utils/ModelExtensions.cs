using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public static string DisplayNumber(this EntityVm venue, EventMetadataVm eventMeta)
        {
            var number = eventMeta.VenueTitleFormatType == VenueTitleFormatType.Name 
                ? null
                : (eventMeta.VenueOrderType == VenueOrderType.Name
                    ? eventMeta.Venues.IndexOf(venue) + 1
                    : (venue.EventDetail != null ? venue.EventDetail.SortOrder : null));
            var result = number.HasValue ? string.Format("{0}. ", number) : string.Empty;
            return result;
        }

        public static string DisplayTime(this ShowVm show, CultureInfo culture)
        {
            var result = DisplayStartTime(show, culture) + DisplayEndTime(show, culture);
            return result;
        }

        public static string DisplayStartTime(this ShowVm show, CultureInfo culture)
        {
            var result = show.StartTime.HasValue && show.EndTime.HasValue
                ? show.StartTime.ToString("t", culture)
                : string.Empty;
            return result;
        }

        public static string DisplayEndTime(this ShowVm show, CultureInfo culture)
        {
            var result = (show.EndTime ?? show.StartTime).ToString("t", culture);
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
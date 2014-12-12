using System;
using System.Collections.Generic;
using System.Linq;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.Core.Utils
{
    public static class ModelExtensions
    {
        private const string Space = " ";

        public static int GetStatus(this OrgEvent orgEvent)
        {
            // current
            if (!orgEvent.StartTime.HasValue ||
                (DateTime.Now.AddDays(-2) <= orgEvent.StartTime &&
                    orgEvent.StartTime <= DateTime.Now.AddDays(2)))
            {
                return 0;
            }

            // past
            if (orgEvent.StartTime < DateTime.Now.AddDays(-2))
            {
                return -1;
            }

            // future
            if (orgEvent.StartTime > DateTime.Now.AddDays(2))
            {
                return 1;
            }

            return -2;
        }

        public static ShowStatus GetStatus(this Show show, Show nextShow = null)
        {
            if (show == null) return default(ShowStatus);

            var startTime = show.StartTime;
            var endTime = show.EndTime ?? (nextShow != null ? nextShow.StartTime : null);

            var status = 
                (startTime.HasValue && startTime.Value.Date != DateTime.Now.Date) ||
                (!startTime.HasValue && endTime >= DateTime.Now) ||
                !endTime.HasValue || endTime >= DateTime.Now
                ? ShowStatus.NotStarted 
                : ShowStatus.Finished;

            if (startTime.HasValue &&
                startTime.Value.Date == DateTime.Now.Date &&
                startTime <= DateTime.Now &&
                (!endTime.HasValue || DateTime.Now <= endTime))
            {
                status = ShowStatus.Started;
            }

            return status;
        }

        public static string GetSearchableText(this object model)
        {
            var result = default(string);

            var venue = model as Venue;
            if (venue != null)
            {
                result = venue.Info.GetSearchableText();
            }

            var show = model as Show;
            if (show != null)
            {
                result = (show.StartTime.HasValue ? Space + show.StartTime : string.Empty) + 
                    (show.EndTime.HasValue ? Space + show.EndTime : string.Empty) +
                    (show.Title != null ? Space + show.Title :  string.Empty) +
                    (show.Description != null ? Space + show.Description :  string.Empty);
            }

            var entity = model as Entity;
            if (entity != null)
            {
                result = (entity.Name != null ? Space + entity.Name : string.Empty) + 
                    (entity.Description != null ? Space + entity.Description : string.Empty) + 
                    (entity.Contacts != null
                        ? Space + entity.Contacts.Select(c => c.GetSearchableText()).Aggregate((a, b) => a + Space + b) 
                        : string.Empty) +
                    (entity.Addresses != null 
                        ? Space + entity.Addresses.Select(a => a.GetSearchableText()).Aggregate((a, b) => a + Space + b) 
                        : string.Empty);
            }

            var contact = model as Contact;
            if (contact != null)
            {
                result = contact.Title ?? string.Empty + Space + 
                    contact.ContactText ?? string.Empty;
            }

            var address = model as Address;
            if (address != null)
            {
                result = address.AddressText ?? string.Empty + Space + 
                    address.Tip ?? string.Empty;
            }

            return result;
        }

        public static bool HasPicture(this Entity info)
        {
            return info != null && info.Picture != null;
        }

        public static bool HasContacts(this Entity entity)
        {
            return entity != null && entity.Contacts != null && entity.Contacts.Length > 0;
        }

        public static bool HasAddresses(this Entity entity)
        {
            return 
                entity != null && 
                entity.Addresses.HasAddresses();
        }

        public static bool HasAddresses(this Address[] addresses)
        {
            return
                addresses != null && 
                addresses.Length > 0 &&
                addresses.Any(a => !a.Latitude.EqualsF(0) && !a.Longitude.EqualsF(0));
        }

        public static bool HasAddressText(this Entity entity)
        {
            return 
                entity != null && 
                entity.Addresses != null && 
                entity.Addresses.Length > 0 &&
                entity.Addresses.Any(a => !string.IsNullOrWhiteSpace(a.AddressText));
        }

        public static string GetAddressText(this Entity entity)
        {
            var text = entity != null 
                ? entity.Addresses.GetAddressText()
                : null;
            return text;
        }

        public static string GetAddressText(this Address[] addresses)
        {
            var text = addresses != null 
                ? addresses
                    .Select(a => a.AddressText)
                    .FirstOrDefault(t => !string.IsNullOrWhiteSpace(t))
                : null;
            return text;
        }

        public static string GetAddressOrCoordText(this Entity entity)
        {
            return entity.Addresses.GetAddressOrCoordText();
        }

        public static string GetAddressOrCoordText(this Address[] addresses)
        {
            var text = GetAddressText(addresses);

            if (addresses.HasAddresses() && text == null)
            {
                var addr = addresses.FirstOrDefault();
                text = string.Format("{0},{1}", addr.Latitude, addr.Longitude);
            }

            return text;
        }

        public static bool HasPicture(this Show show)
        {
            return show != null && show.Picture != null;
        }

        public static bool HasDetailsUrl(this Show show)
        {
            return show != null && show.DetailsUrl != null;
        }

        public static int Id(this Reference[] refs)
        {
            var smartWalkRef = refs != null 
                ? refs.FirstOrDefault(r => r.Storage == Storage.SmartWalk) 
                : null;
            return smartWalkRef != null ? smartWalkRef.Id : 0;
        }

        public static Venue[] OrderBy(
            this IEnumerable<Venue> venues, 
            EventVenueDetail[] venueDetails,
            VenueOrderType? orderType)
        {
            Venue[] result;

            if (orderType == VenueOrderType.Custom)
            {
                result = venues
                    .OrderBy(v =>
                        {
                            var venueDetail = venueDetails
                                .FirstOrDefault(vd => vd.Venue.Id() == v.Info.Id);
                            return venueDetail != null && venueDetail.SortOrder != null
                                ? venueDetail.SortOrder
                                : 0;
                        })
                    .ToArray();
            }
            else if (orderType == VenueOrderType.Name)
            {
                result = venues
                    .OrderBy(v => v.Info.Name, StringComparer.CurrentCulture)
                    .ToArray();
            }
            else
            {
                result = venues.ToArray();
            }

            return result;
        }

        public static string DisplayName(this Venue venue)
        {
            var result = venue != null && venue.Info != null
                ? (venue.Number != null 
                    ? string.Format("{0}. {1}", venue.Number, venue.Info.Name)
                    : venue.Info.Name)
                : null;
            return result;
        }

        public static string PinText(this Venue venue)
        {
            var result = venue != null && venue.Info != null
                ? PinText(venue.Number, venue.Info.Name)
                : null;
            return result;
        }

        public static string PinText(int? number, string name)
        {
            var result = number != null 
                ? number.ToString()
                : name.GetAbbreviation(2);
            return result;
        }

        public static string GetDateString(this OrgEvent orgEvent)
        {
            var result = orgEvent != null && orgEvent.StartTime.HasValue
                ? string.Format(
                    "{0:D}{1}{2:D}", 
                    orgEvent.StartTime, 
                    orgEvent.EndTime.HasValue ? " - " : string.Empty, 
                    orgEvent.EndTime)
                : null;
            return result;
        }

        public static string GetCurrentDayString(this DateTime date, string empty = null)
        {
            var result = date != DateTime.MinValue
                ? string.Format("{0:ddd, d MMMM}", date)
                : empty;
            return result;
        }

        public static string GetCurrentDayString(this DateTime? date, string empty = null)
        {
            var result = date.HasValue 
                ? date.Value.GetCurrentDayString(empty) 
                : null;
            return result;
        }

        public static Venue GetVenueByShow(this Venue[] venues, Show show)
        {
            if (venues != null && show != null)
            {
                return 
                    venues
                        .FirstOrDefault(v => v.Info.Id == show.Venue
                            .First(r => r.Storage == Storage.SmartWalk).Id);
            }

            return null;
        }

        public static Tuple<DateTime?, DateTime?> GetOrgEventRange(this OrgEvent orgEvent)
        {
            var result = orgEvent != null && orgEvent.StartTime.HasValue
                ? new Tuple<DateTime?, DateTime?>(orgEvent.StartTime, orgEvent.EndTime)
                : null;
            return result;
        }

        public static Dictionary<DateTime, Show[]> GroupByDay(this Show[] shows)
        {
            if (shows == null || shows.Length == 0) return null;

            var orderedShows = shows.OrderBy(s => s.StartTime).ToArray();
            var day = (orderedShows.First().StartTime ?? DateTime.MinValue).Date;

            var result = new Dictionary<DateTime, List<Show>>();
            result[day] = new List<Show>();

            foreach (var show in orderedShows)
            {
                if (show.StartTime.HasValue && !show.StartTime.IsTimeThisDay(day))
                {
                    day = show.StartTime.Value.Date;
                    result[day] = new List<Show>();
                }

                result[day].Add(show);
            }

            return result.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
        }

        public static Show[] GroupByDayShow(this Show[] shows)
        {
            var groupes = GroupByDay(shows);
            if (groupes == null) return shows;

            var venue = shows[0].Venue;
            var result = new List<Show>();

            foreach (var day in groupes.Keys)
            {
                if (day != DateTime.MinValue)
                {
                    result.Add(GetDayGroupShow(day, venue));
                }

                result.AddRange(groupes[day]);
            }

            return result.ToArray();
        }

        public static Venue[] GroupByDayVenue(this Show[] shows)
        {
            var groupes = GroupByDay(shows);
            if (groupes == null) return null;

            var result = new List<Venue>();

            foreach (var day in groupes.Keys)
            {
                var groupVenue = GetDayGroupVenue(day);
                groupVenue.Shows = groupes[day];
                result.Add(groupVenue);
            }

            return result.ToArray();
        }

        private static Show GetDayGroupShow(DateTime day, Reference[] venue)
        {
            return new Show 
            {
                Id = Show.DayGroupId,
                StartTime = day,
                Venue = venue
            };
        }

        private static Venue GetDayGroupVenue(DateTime day)
        {
            return new Venue(
                new Entity {
                    Id = Venue.DayGroupId,
                    Name = day.GetCurrentDayString()
                },
                null);
        }
    }

    public enum ShowStatus
    {
        NotStarted,
        Started,
        Finished
    }
}
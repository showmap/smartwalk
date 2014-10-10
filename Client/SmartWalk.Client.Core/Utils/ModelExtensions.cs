using System;
using System.Linq;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using System.Collections.Generic;
using SmartWalk.Shared.Utils;

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

        public static ShowStatus GetStatus(this Show show)
        {
            var status = 
                (show.StartTime.HasValue && show.StartTime.Value.Date != DateTime.Now.Date) ||
                (!show.StartTime.HasValue && show.EndTime >= DateTime.Now) ||
                !show.EndTime.HasValue || 
                show.EndTime >= DateTime.Now
                ? ShowStatus.NotStarted 
                : ShowStatus.Finished;

            if (show.StartTime.HasValue &&
                show.StartTime.Value.Date == DateTime.Now.Date &&
                show.StartTime <= DateTime.Now &&
                (!show.EndTime.HasValue || DateTime.Now <= show.EndTime))
            {
                status = ShowStatus.Started;
            }

            return status;
        }

        public static string GetText(this Show show)
        {
            var result = show.Title + 
                (show.Description != null ? Space + show.Description : string.Empty);
            return result;
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
                entity.Addresses != null && 
                entity.Addresses.Length > 0 &&
                // Analysis disable RedundantCast
                entity.Addresses.Any(a => (int)a.Latitude != 0 && (int)a.Longitude != 0);
                // Analysis enable RedundantCast;
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

        public static string GetCurrentDayString(this DateTime date)
        {
            var result = string.Format("{0:ddd, d MMMM}", date);
            return result;
        }

        public static string GetCurrentDayString(this DateTime? date)
        {
            var result = date.HasValue 
                ? date.Value.GetCurrentDayString() 
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

        public static Dictionary<DateTime, Show[]> GroupByDay(
            this Show[] shows,
            Tuple<DateTime?, DateTime?> range)
        {
            if (shows == null || 
                shows.Length == 0 || 
                !shows.Any(s => s.StartTime.HasValue)) return null;

            var orderedShows = shows.OrderBy(s => s.StartTime).ToArray();
            var firstDay = orderedShows
                .FirstOrDefault(s => s.StartTime.HasValue)
                .StartTime.Value.Date;
            var day = firstDay;

            var result = new Dictionary<DateTime, List<Show>>();
            result[day] = new List<Show>();

            foreach (var show in orderedShows)
            {
                if (show.StartTime.HasValue &&
                    !show.StartTime.IsTimeThisDay(day, range))
                {
                    day = show.StartTime.Value.Date;
                    result[day] = new List<Show>();
                }

                result[day].Add(show);
            }

            return result.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
        }

        public static Show[] GroupByDayShow(this Show[] shows, Tuple<DateTime?, DateTime?> range)
        {
            var groupes = GroupByDay(shows, range);
            if (groupes == null) return shows;

            var venue = shows[0].Venue;
            var result = new List<Show>();

            foreach (var day in groupes.Keys)
            {
                result.Add(GetDayGroupShow(day, venue));
                result.AddRange(groupes[day]);
            }

            return result.ToArray();
        }

        public static Venue[] GroupByDayVenue(this Show[] shows, Tuple<DateTime?, DateTime?> range)
        {
            var groupes = GroupByDay(shows, range);
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
            return new Venue(new Entity {
                Id = Venue.DayGroupId,
                Name = day.GetCurrentDayString()
            });
        }
    }

    public enum ShowStatus
    {
        NotStarted,
        Started,
        Finished
    }
}
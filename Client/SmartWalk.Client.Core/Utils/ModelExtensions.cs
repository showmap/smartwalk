using System;
using System.Linq;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.Core.Utils
{
    public static class ModelExtensions
    {
        private const string Space = " ";

        public static int GetStatus(this OrgEvent orgEvent)
        {
            if (orgEvent.StartTime < DateTime.Now.AddDays(-2))
            {
                return -1;
            }

            if (DateTime.Now.AddDays(-2) <= orgEvent.StartTime &&
                orgEvent.StartTime <= DateTime.Now.AddDays(2))
            {
                return 0;
            }

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
            return entity != null && entity.Addresses != null && entity.Addresses.Length > 0;
        }

        public static int Id(this Reference[] refs)
        {
            var smartWalkRef = refs != null 
                ? refs.FirstOrDefault(r => r.Storage == Storage.SmartWalk) 
                : null;
            return smartWalkRef != null ? smartWalkRef.Id : 0;
        }
    }

    public enum ShowStatus
    {
        NotStarted,
        Started,
        Finished
    }
}
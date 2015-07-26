using System;
using System.Linq;
using System.Collections.Generic;

namespace SmartWalk.Client.Core.Model
{
    public class Favorites
    {
        public Favorites()
        {
            Events = new List<FavoriteEvent>();
        }

        public List<FavoriteEvent> Events { get; set; }

        public DateTime LastUpdated { get; set; } 
    }

    public class FavoriteEvent
    {
        public FavoriteEvent()
        {
            ShowIds = new List<int>();
        }

        public int Id { get; set; }
        public List<int> ShowIds { get; set; }
    }

    public static class FavoritesExtensions 
    {
        public static FavoriteEvent GetEvent(this Favorites favorites, int orgEventId)
        {
            var result = favorites.Events
                .FirstOrDefault(e => e.Id == orgEventId);
            if (result == null)
            {
                result = new FavoriteEvent { Id = orgEventId };
                favorites.Events.Add(result);
            }

            return result;
        }

        public static bool AddEventShow(this Favorites favorites, int orgEventId, int showId)
        {
            var orgEvent = favorites.GetEvent(orgEventId);
            if (!orgEvent.ShowIds.Contains(showId))
            {
                orgEvent.ShowIds.Add(showId);
                return true;
            }

            return false;
        }

        public static bool RemoveEventShow(this Favorites favorites, int orgEventId, int showId)
        {
            var orgEvent = favorites.GetEvent(orgEventId);
            if (orgEvent.ShowIds.Contains(showId))
            {
                orgEvent.ShowIds.Remove(showId);
                return true;
            }

            return false;
        }
    }
}
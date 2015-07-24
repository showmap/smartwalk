using System;
using System.Linq;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels.Common;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class VenueShowDataContext
    {
        public VenueShowDataContext(Show show, 
            FavoritesShowManager favoritesManager,
            bool isLogoVisible = true,
            OrgEvent orgEvent = null,
            bool isGroupedByLocation = true,
            bool isTimeVisible = true)
        {
            if (!isGroupedByLocation && orgEvent != null)
            {
                var groupedShow = show as GroupedShow;
                if (groupedShow != null)
                {
                    Venues = orgEvent.Venues.GetVenuesByGroupedShow(groupedShow);
                }
                else
                {
                    Venue = orgEvent.Venues.GetVenueByShow(show);
                }
            }

            Show = show;

            IsFavorite = favoritesManager.IsShowFavorite(show);
            IsLogoVisible = isLogoVisible;
            IsTimeVisible = isTimeVisible;
        }

        public Venue Venue { get; private set; }
        public Venue[] Venues { get; private set; }
        public Show Show { get; private set; }
        public bool IsFavorite { get; set; }
        public bool IsLogoVisible { get; private set; }
        public bool IsTimeVisible { get; private set; }

        public bool IsLocationAvailable
        {
            get { return TargetVenue != null; }
        }

        public Venue TargetVenue
        {
            get { return Venue ?? (Venues != null ? Venues.FirstOrDefault() : null); }
        }

        public string GetLocationString()
        {
            var result = default(string);

            if (Venue != null)
            {
                result = Venue.DisplayName();
            }

            if (Venues != null)
            {
                result = Venues.Aggregate(string.Empty, 
                    (prev, v) => prev + 
                    (prev != string.Empty ? ", " : string.Empty) + v.DisplayName());
            }

            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cirrious.CrossCore.Converters;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.ViewModels;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventTableSourceConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var venues = value as Venue[];
            var viewModel = parameter as OrgEventViewModel;

            if (venues != null &&
                viewModel != null &&
                viewModel.IsMultiday &&
                viewModel.IsGroupedByLocation &&
                !viewModel.CurrentDay.HasValue)
            {
                var result = 
                    venues.Select(
                        v => new Venue(v.Info) {
                            Shows = GetShowsGroupedByDay(v.Shows)
                    }).ToArray();
                return result;
            }

            return venues;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static Show[] GetShowsGroupedByDay(Show[] shows)
        {
            if (shows == null || 
                shows.Length == 0 || 
                !shows.Any(s => s.StartTime.HasValue)) return shows;

            var venue = shows[0].Venue;
            var orderedShows = shows.OrderBy(s => s.StartTime).ToArray();
            var day = orderedShows
                .FirstOrDefault(s => s.StartTime.HasValue)
                .StartTime.Value.Date;

            var result = new List<Show>();
            result.Add(GetDayBlankShow(day, venue));

            foreach (var show in orderedShows)
            {
                if (show.StartTime.HasValue &&
                    show.StartTime.Value.Date != day)
                {
                    day = show.StartTime.Value.Date;
                    result.Add(GetDayBlankShow(day, venue));
                }

                result.Add(show);
            }

            return result.ToArray();
        }

        private static Show GetDayBlankShow(DateTime day, Reference[] venue)
        {
            return new Show 
                {
                    Id = DayBlankShow.Id,
                    StartTime = day,
                    Venue = venue
                };
        }
    }

    public static class DayBlankShow
    {
        public const int Id = -1000;
    }
}
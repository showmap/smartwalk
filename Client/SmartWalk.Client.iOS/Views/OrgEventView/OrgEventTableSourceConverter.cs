using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cirrious.CrossCore.Converters;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;
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
                            Shows = DayHeaderShow.GetShowsWithDayHeaders(v.Shows)
                    }).ToArray();
                return result;
            }

            return venues;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class DayHeaderShow
    {
        public const int Id = -1000;

        public static Dictionary<DateTime, Show[]> GetShowsGroupedByDay(Show[] shows)
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
                    !show.IsShowThisDay(day, firstDay))
                {
                    day = show.StartTime.Value.Date;
                    result[day] = new List<Show>();
                }

                result[day].Add(show);
            }

            return result.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
        }

        public static Show[] GetShowsWithDayHeaders(Show[] shows)
        {
            var groupes = GetShowsGroupedByDay(shows);
            if (groupes == null) return shows;

            var venue = shows[0].Venue;
            var result = new List<Show>();

            foreach (var day in groupes.Keys)
            {
                result.Add(GetDayBlankShow(day, venue));
                result.AddRange(groupes[day]);
            }

            return result.ToArray();
        }

        public static Show GetDayBlankShow(DateTime day, Reference[] venue)
        {
            return new Show 
                {
                    Id = DayHeaderShow.Id,
                    StartTime = day,
                    Venue = venue
                };
        }
    }
}
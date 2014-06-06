using System;
using System.Globalization;
using System.Linq;
using Cirrious.CrossCore.Converters;
using SmartWalk.Client.Core.Model;
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
                !viewModel.CurrentDay.HasValue)
            {
                if (viewModel.IsGroupedByLocation)
                {
                    var result = 
                        venues.Select(
                            v => new Venue(v.Info) {
                                Shows = v.Shows.GroupByDayShow()
                            }).ToArray();
                    return result;
                }

                if (viewModel.SortBy == SortBy.Time &&
                      venues.Length > 0)
                {
                    var fooVenue = venues[0]; // expecting foo venue that holds all shows
                    var result = fooVenue.Shows.GroupByDayVenue();
                    return result;
                }
            }

            return venues;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
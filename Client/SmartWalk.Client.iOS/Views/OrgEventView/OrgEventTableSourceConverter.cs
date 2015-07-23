using System;
using System.Globalization;
using System.Linq;
using Cirrious.CrossCore.Converters;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventTableSourceConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value as Venue[];
            var viewModel = parameter as OrgEventViewModel;

            if (result != null &&
                viewModel != null &&
                viewModel.IsMultiday &&
                !viewModel.CurrentDay.HasValue)
            {
                if (viewModel.IsGroupedByLocation)
                {
                    result = result
                        .Select(v => 
                            new Venue(v.Info, v.Description) {
                                Number = v.Number,
                                Shows = v.Shows.GroupByDayShow()
                        })
                        .ToArray();
                }
                else if (viewModel.SortBy == SortBy.Name && result.Length > 0)
                {
                    var fooVenue = result[0]; // expecting foo venue that holds all shows
                    result = fooVenue.Shows.GroupTitleDuplicates();
                }
                else if (viewModel.SortBy == SortBy.Time && result.Length > 0)
                {
                    var fooVenue = result[0]; // expecting foo venue that holds all shows
                    result = fooVenue.Shows.GroupByDayVenue();
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
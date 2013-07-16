using System;
using System.Collections.Generic;
using System.Globalization;
using Cirrious.CrossCore.Converters;
using SmartWalk.Core.ViewModels;

namespace SmartWalk.iOS.Converters
{
    public class VenueAndShowsTableSourceConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var venueViewModel = value as VenueViewModel;
            if (venueViewModel != null)
            {
                var result = new List<GroupContainer>();

                result.Add(new GroupContainer(new [] { venueViewModel }));

                result.Add(
                    new GroupContainer(venueViewModel.Venue.Shows) 
                    {
                        Key = "Shows"
                    });

                return result.ToArray();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
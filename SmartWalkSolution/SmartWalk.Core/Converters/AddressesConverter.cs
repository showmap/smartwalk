using System;
using System.Globalization;
using Cirrious.CrossCore.Converters;
using SmartWalk.Core.Model;

namespace SmartWalk.Core.Converters
{
    public class AddressesConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var addressses = value as AddressInfo[];
            if (addressses != null && addressses.Length > 0)
            {
                return addressses[0].Address;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
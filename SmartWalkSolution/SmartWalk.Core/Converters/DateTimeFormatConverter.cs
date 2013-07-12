using System;
using System.Globalization;
using Cirrious.CrossCore.Converters;

namespace SmartWalk.Core.Converters
{
    public class DateTimeFormatConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var format = parameter as string;
            if (value is DateTime && format != null)
            {
                var dateTime = (DateTime)value;
                return String.Format("{0:" + format + "}", dateTime);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
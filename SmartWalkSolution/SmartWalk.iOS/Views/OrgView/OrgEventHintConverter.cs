using System;
using Cirrious.CrossCore.Converters;
using System.Globalization;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Views.OrgView
{
    public class OrgEventHintConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var eventInfo = value as OrgEventInfo;
            if (eventInfo != null)
            {
                return eventInfo.HasSchedule ? null : "(no schedule)";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
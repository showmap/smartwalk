using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cirrious.CrossCore.Converters;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Views.Converters
{
    public class OrgTableSourceConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var org = value as Org;
            if (org != null)
            {
                var result = new List<GroupContainer>();

                result.Add(new GroupContainer(new [] { org }));

                var pastEvents = org.EventInfos
                    .Where(ei => ei.TimeStatus < 0).ToArray();
                if (pastEvents.Length > 0)
                {
                    result.Add(
                        new GroupContainer(pastEvents) 
                        {
                        Key = "Past Events"
                    });
                }

                var currentEvents = org.EventInfos
                    .Where(ei => ei.TimeStatus == 0).ToArray();
                if (currentEvents.Length > 0)
                {
                    result.Add(
                        new GroupContainer(currentEvents) 
                        {
                        Key = "Current Events"
                    });
                }

                var futureEvents = org.EventInfos
                    .Where(ei => ei.TimeStatus > 0).ToArray();
                if (futureEvents.Length > 0)
                {
                    result.Add(
                        new GroupContainer(futureEvents) 
                        {
                        Key = "Future Events"
                    });
                }

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
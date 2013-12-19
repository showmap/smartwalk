using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cirrious.CrossCore.Converters;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Views.Common;

namespace SmartWalk.Client.iOS.Views.OrgView
{
    public class OrgTableSourceConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var org = value as Org;
            var orgViewModel = parameter as OrgViewModel;
            if (org != null && orgViewModel != null)
            {
                var result = new List<GroupContainer>();

                result.Add(new GroupContainer(new [] { new EntityViewModelWrapper(orgViewModel) }));

                var currentEvents = org.EventInfos
                    .Where(ei => ei.TimeStatus == 0).OrderBy(ei => ei.Date).ToArray();
                if (currentEvents.Length > 0)
                {
                    result.Add(
                        new GroupContainer(currentEvents) 
                        {
                            Key = "Current Events"
                        });
                }

                var futureEvents = org.EventInfos
                    .Where(ei => ei.TimeStatus > 0).OrderBy(ei => ei.Date).ToArray();
                if (futureEvents.Length > 0)
                {
                    result.Add(
                        new GroupContainer(futureEvents) 
                        {
                            Key = "Upcoming Events"
                        });
                }

                var pastEvents = org.EventInfos
                    .Where(ei => ei.TimeStatus < 0).OrderByDescending(ei => ei.Date).ToArray();
                if (pastEvents.Length > 0)
                {
                    result.Add(
                        new GroupContainer(pastEvents) 
                        {
                            Key = "Past Events"
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
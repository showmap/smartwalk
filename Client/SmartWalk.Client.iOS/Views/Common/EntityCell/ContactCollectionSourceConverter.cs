using System;
using System.Collections.Generic;
using System.Globalization;
using Cirrious.CrossCore.Converters;
using SmartWalk.Client.Core.Model;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    public class ContactCollectionSourceConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var contactInfo = value as ContactInfo;
            if (contactInfo != null)
            {
                var result = new List<object>();

                if (contactInfo.Emails != null)
                {
                    result.AddRange(contactInfo.Emails);
                }

                if (contactInfo.Phones != null)
                {
                    result.AddRange(contactInfo.Phones);
                }

                if (contactInfo.WebSites != null)
                {
                    result.AddRange(contactInfo.WebSites);
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
using System;
using System.Globalization;
using Cirrious.CrossCore.Converters;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Views.Common.EntityCell;

namespace SmartWalk.Client.iOS.Views.OrgEventInfoView
{
    public class OrgEventInfoTableSourceConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var org = value as OrgEvent;
            var viewModel = parameter as OrgEventInfoViewModel;
            if (org != null && viewModel != null)
            {
                var result = new [] 
                    { 
                        new EntityViewModelWrapper(viewModel) 
                    };
                return result;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
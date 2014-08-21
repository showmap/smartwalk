using System;
using System.Collections.Generic;
using System.Globalization;
using Cirrious.CrossCore.Converters;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Views.Common.EntityCell;
using SmartWalk.Client.iOS.Views.Common.GroupHeader;

namespace SmartWalk.Client.iOS.Views.VenueView
{
    public class VenueTableSourceConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var venue = value as Venue;
            var viewModel = parameter as VenueViewModel;
            if (venue != null && viewModel != null)
            {
                var result = new List<GroupContainer>();

                result.Add(new GroupContainer(new [] { 
                    new EntityViewModelWrapper(
                        viewModel, 
                        EntityViewModelWrapper.ModelMode.Venue) 
                }));

                if (venue.Shows != null &&
                    venue.Shows.Length > 0)
                {
                    var groupes = venue.Shows.GroupByDay(
                        viewModel.OrgEvent.GetOrgEventRange());

                    result.Add(new GroupContainer(
                        groupes != null 
                            ? new object[]{ } 
                            : venue.Shows) 
                        { 
                            Key = Localization.Shows 
                        });

                    if (groupes != null)
                    {
                        foreach (var day in groupes.Keys)
                        {
                            result.Add(new GroupContainer(groupes[day]) {
                                Key = day.GetCurrentDayString()
                            });
                        }
                    }
                }

                if (viewModel.CanShowNextEntity)
                {
                    result.Add(new GroupContainer(new [] { 
                        viewModel.NextEntityTitle
                    }));
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
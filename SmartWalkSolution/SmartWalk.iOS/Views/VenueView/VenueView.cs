using System.ComponentModel;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Controls;
using SmartWalk.iOS.Views.Common;
using SmartWalk.iOS.Views.OrgEventView;

namespace SmartWalk.iOS.Views.VenueView
{
    public partial class VenueView : ListViewBase
    {
        public new VenueViewModel ViewModel
        {
            get { return (VenueViewModel)base.ViewModel; }
        }

        protected override void UpdateViewTitle()
        {
            if (ViewModel.Venue != null && ViewModel.Venue.Info != null)
            {
                NavigationItem.Title = ViewModel.Venue.Info.Name;
            }
        }

        protected override ListViewDecorator GetListView()
        { 
            return new ListViewDecorator(VenueShowsTableView);  
        }

        protected override object CreateListViewSource()
        {
            var tableSource = new VenueTableSource(VenueShowsTableView, ViewModel);

            this.CreateBinding(tableSource).To((VenueViewModel vm) => vm)
                .WithConversion(new VenueTableSourceConverter(), null).Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                VenueShowsTableView.BeginUpdates();
                VenueShowsTableView.EndUpdates();
            }
            else if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.ExpandedShow))
            {
                VenueShowCell.SetVenueCellsTableIsResizing(VenueShowsTableView, true);
                VenueShowCell.CollapseVenueShowCell(VenueShowsTableView);

                CATransaction.Begin();
                CATransaction.CompletionBlock = new NSAction(
                    () => VenueShowCell.SetVenueCellsTableIsResizing(VenueShowsTableView, false));

                VenueShowsTableView.BeginUpdates();
                VenueShowsTableView.EndUpdates();

                CATransaction.Commit();

                VenueShowCell.ExpandVenueShowCell(VenueShowsTableView, ViewModel.ExpandedShow);
            }
        }
    }
}
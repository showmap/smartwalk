using System.ComponentModel;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.UIKit;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Converters;

namespace SmartWalk.iOS.Views
{
    public partial class VenueView : TableViewBase
    {
        public new VenueViewModel ViewModel
        {
            get { return (VenueViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override UITableView TableView { get { return VenueShowsTableView; } }

        protected override void UpdateViewTitle()
        {
            if (ViewModel.Venue != null && ViewModel.Venue.Info != null)
            {
                NavigationItem.Title = ViewModel.Venue.Info.Name;
            }
        }

        protected override MvxTableViewSource CreateTableViewSource()
        {
            var tableSource = new VenueTableSource(VenueShowsTableView, ViewModel);

            this.CreateBinding(tableSource).To((VenueViewModel vm) => vm)
                .WithConversion(new VenueAndShowsTableSourceConverter(), null).Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                VenueShowsTableView.BeginUpdates();
                VenueShowsTableView.EndUpdates();
            }
        }
    }
}
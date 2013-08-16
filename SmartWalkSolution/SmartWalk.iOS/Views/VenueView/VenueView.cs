using System.ComponentModel;
using System.Linq;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.ViewModels;
using MonoTouch.UIKit;
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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeToolBar();
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseDesignerOutlets();
            base.Dispose(disposing);
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

            tableSource.ShowImageFullscreenCommand = new MvxCommand<string>(ShowImageFullscreenView);

            this.CreateBinding(tableSource).To((VenueViewModel vm) => vm.Venue)
                .WithConversion(new VenueTableSourceConverter(), ViewModel).Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.Venue))
            {
                UpdateViewTitle();
            }
            else if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                VenueShowsTableView.BeginUpdates();
                VenueShowsTableView.EndUpdates();
            }
            else if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.ExpandedShow))
            {
                foreach (var cell in VenueShowsTableView.VisibleCells.OfType<VenueShowCell>())
                {
                    cell.IsExpanded = Equals(cell.DataContext, ViewModel.ExpandedShow);
                }

                VenueShowsTableView.BeginUpdates();
                VenueShowsTableView.EndUpdates();
            }
        }

        private void InitializeToolBar()
        {
            var buttonUp = new UIBarButtonItem("↑", UIBarButtonItemStyle.Plain, (s, e) => 
                {
                    if (ViewModel.ShowPreviousEntityCommand.CanExecute(null))
                    {
                        ViewModel.ShowPreviousEntityCommand.Execute(null);
                    }
                });
            var buttonDown = new UIBarButtonItem("↓", UIBarButtonItemStyle.Plain, (s, e) => 
                {
                    if (ViewModel.ShowNextEntityCommand.CanExecute(null))
                    {
                        ViewModel.ShowNextEntityCommand.Execute(null);
                    }
                });

            NavigationItem.SetRightBarButtonItems(new [] {buttonDown, buttonUp}, true);
        }
    }
}
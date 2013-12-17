using System;
using System.Linq;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Controls;
using SmartWalk.iOS.Utils;
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

        protected override string GetViewTitle()
        {
            if (ViewModel.Venue != null && ViewModel.Venue.Info != null)
            {
                return ViewModel.Venue.Info.Name;
            }

            return null;
        }

        protected override ListViewDecorator GetListView()
        { 
            return new ListViewDecorator(VenueShowsTableView);  
        }

        protected override UIView GetProgressViewContainer()
        { 
            return ProgressViewContainer;  
        }

        protected override IListViewSource CreateListViewSource()
        {
            var tableSource = new VenueTableSource(VenueShowsTableView, ViewModel);

            this.CreateBinding(tableSource).To((VenueViewModel vm) => vm.Venue)
                .WithConversion(new VenueTableSourceConverter(), ViewModel).Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            if (propertyName == ViewModel.GetPropertyName(vm => vm.Venue))
            {
                GetViewTitle();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                VenueShowsTableView.BeginUpdates();
                VenueShowsTableView.EndUpdates();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.ExpandedShow))
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
            var rightBarButtons = ButtonBarUtil.GetUpDownBarItems(
                new Action(() => {
                    if (ViewModel.ShowPreviousEntityCommand.CanExecute(null))
                    {
                        ViewModel.ShowPreviousEntityCommand.Execute(null);
                    }
                }),
                new Action(() => {
                    if (ViewModel.ShowNextEntityCommand.CanExecute(null))
                    {
                        ViewModel.ShowNextEntityCommand.Execute(null);
                    }
                }));
            NavigationItem.SetRightBarButtonItems(rightBarButtons, true);
        }
    }
}
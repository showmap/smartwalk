using System.ComponentModel;
using System.Linq;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Utils;
using SmartWalk.iOS.Views.Cells;

namespace SmartWalk.iOS.Views
{
    public partial class OrgEventView : TableViewBase
    {
        private UIBarButtonItem _modeButton;

        public new OrgEventViewModel ViewModel
        {
            get { return (OrgEventViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override UITableView TableView { get { return VenuesAndShowsTableView; } }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (ViewModel.SwitchModeCommand.CanExecute(OrgEventViewMode.List))
            {
                ViewModel.SwitchModeCommand.Execute(OrgEventViewMode.List);
            }

            InitializeToolBar();
            InitializeMapView();

            UpdateViewModeState();
        }

        protected override void UpdateViewTitle()
        {
            if (ViewModel.OrgEvent != null && ViewModel.OrgEvent.Info != null)
            {
                NavigationItem.Title = ViewModel.OrgEvent.Info.Date.ToShortDateString();
            }
        }

        protected override MvxTableViewSource CreateTableViewSource()
        {
            var tableSource = new VenuesAndShowsTableSource(VenuesAndShowsTableView, ViewModel);

            this.CreateBinding(tableSource).To((OrgEventViewModel vm) => vm.OrgEvent.Venues).Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.OrgEvent))
            {
                InitializeMapView();
            }
            else if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.Mode))
            {
                UpdateViewModeState();
            }
        }

        private void InitializeToolBar()
        {
            _modeButton = new UIBarButtonItem();

            _modeButton.Clicked += (sender, e) => 
                {
                    if (ViewModel.SwitchModeCommand.CanExecute(null))
                    {
                        ViewModel.SwitchModeCommand.Execute(null);
                    }
                };

            NavigationItem.SetRightBarButtonItem(_modeButton, true);
        }

        private void InitializeMapView()
        {
            if (ViewModel.OrgEvent != null &&
                ViewModel.OrgEvent.Venues != null)
            {
                var annotations = ViewModel.OrgEvent.Venues
                    .SelectMany(v => v.Info.Addresses
                        .Select(a => new VenueAnnotation(v.Number, v.Info, a))).ToArray();
                var coordinates = annotations
                    .Select(va => va.Coordinate)
                    .Where(c => c.Latitude != 0 && c.Longitude != 0).ToArray();

                VenuesMapView.SetRegion(MapUtil.CoordinateRegionForCoordinates(coordinates), false);
                VenuesMapView.AddAnnotations(annotations);
            }
        }
       
        private void UpdateViewModeState()
        {
            if (ViewModel.Mode == OrgEventViewMode.Map)
            {
                TablePanel.Hidden = true;
                MapPanel.Hidden = false;
                _modeButton.Title = "List";
            }
            else
            {
                TablePanel.Hidden = false;
                MapPanel.Hidden = true;
                _modeButton.Title = "Map";
            }
        }
    }

    public class VenuesAndShowsTableSource : MvxTableViewSource
    {
        private readonly OrgEventViewModel _viewModel;
        private readonly ViewsFactory<VenueCell> _headerViewFactory;

        public VenuesAndShowsTableSource(UITableView tableView, OrgEventViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;
            _headerViewFactory = new ViewsFactory<VenueCell>(VenueCell.Create, 7);

            UseAnimations = true;

            tableView.RegisterNibForCellReuse(VenueShowCell.Nib, VenueShowCell.Key);
        }

        public Venue[] VenueItemsSource
        {
            get { return ItemsSource != null ? (Venue[])ItemsSource : null; }
        }

        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            return 76.0f;
        }

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 35.0f;
        }

        public override int NumberOfSections(UITableView tableView)
        {
            return VenueItemsSource != null ? VenueItemsSource.Count() : 0;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return VenueItemsSource != null && VenueItemsSource[section].Shows != null 
                ? VenueItemsSource[section].Shows.Count() 
                : 0;
        }

        public override UIView GetViewForHeader(UITableView tableView, int section)
        {
            var headerView = _headerViewFactory.DequeueReusableView();

            headerView.DataContext = VenueItemsSource[section];
            headerView.NavigateVenueCommand = _viewModel.NavigateVenueCommand;
            headerView.NavigateVenueOnMapCommand = _viewModel.NavigateVenueOnMapCommand;

            return headerView;
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            var cell = tableView.DequeueReusableCell(VenueShowCell.Key, indexPath);
            return cell;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            return VenueItemsSource != null && VenueItemsSource[indexPath.Section].Shows != null 
                ? VenueItemsSource[indexPath.Section].Shows.ElementAt(indexPath.Row) 
                : null;
        }
    }
}
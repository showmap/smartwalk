using System.ComponentModel;
using System.Linq;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Touch.Views;
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
    public partial class OrgEventView : MvxViewController
    {
        private UIRefreshControl _refreshControl;
        private UIBarButtonItem _modeButton;

        public new OrgEventViewModel ViewModel
        {
            get { return (OrgEventViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;

            UpdateViewTitle();

            InitializeToolBar();
            InitializeTableView();
            InitializeMapView();

            UpdateViewModeState();
        }

        private void UpdateViewTitle()
        {
            if (ViewModel.OrgEvent != null && ViewModel.OrgEvent.Info != null)
            {
                NavigationItem.Title = ViewModel.OrgEvent.Info.Date.ToShortDateString();
            }
        }

        private void InitializeToolBar()
        {
            _modeButton = new UIBarButtonItem();

            this.CreateBinding(_modeButton).To((OrgEventViewModel vm) => vm.SwitchModeCommand).Apply();

            NavigationItem.SetRightBarButtonItem(_modeButton, true);
        }

        private void InitializeTableView()
        {
            var tableSource = new VenuesAndShowsTableSource(VenuesAndShowsTableView, ViewModel);

            this.CreateBinding(tableSource).To((OrgEventViewModel vm) => vm.OrgEvent.Venues).Apply();

            VenuesAndShowsTableView.Source = tableSource;
            VenuesAndShowsTableView.ReloadData();

            _refreshControl = new UIRefreshControl();
            _refreshControl.ValueChanged += (sender, e) => 
                {
                    if (ViewModel.RefreshCommand.CanExecute(null))
                    {
                        ViewModel.RefreshCommand.Execute(null);
                    }
                };

            VenuesAndShowsTableView.AddSubview(_refreshControl);
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

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.OrgEvent))
            {
                UpdateViewTitle();
                InitializeMapView();
                InvokeOnMainThread(_refreshControl.EndRefreshing);
            }
            else if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.Mode))
            {
                UpdateViewModeState();
            }
        }

        private void UpdateViewModeState()
        {
            if (ViewModel.Mode == OrgViewMode.Map)
            {
                TableView.Hidden = true;
                MapView.Hidden = false;
                _modeButton.Title = "List";
            }
            else
            {
                TableView.Hidden = false;
                MapView.Hidden = true;
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
            _headerViewFactory = new ViewsFactory<VenueCell>(VenueCell.Create);

            UseAnimations = true;

            tableView.RegisterNibForCellReuse(VenueShowCell.Nib, VenueShowCell.Key);
        }

        public Venue[] VenueItemsSource
        {
            get { return ItemsSource != null ? (Venue[])ItemsSource : null; }
        }

        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            return 70.0f;
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
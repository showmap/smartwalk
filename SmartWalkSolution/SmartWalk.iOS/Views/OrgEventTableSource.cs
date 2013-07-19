using System.Linq;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Utils;
using SmartWalk.iOS.Views.Cells;

namespace SmartWalk.iOS.Views
{
    public class OrgEventTableSource : MvxTableViewSource
    {
        private readonly OrgEventViewModel _viewModel;
        private readonly ViewsFactory<VenueCell> _headerViewFactory;

        public OrgEventTableSource(UITableView tableView, OrgEventViewModel viewModel)
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
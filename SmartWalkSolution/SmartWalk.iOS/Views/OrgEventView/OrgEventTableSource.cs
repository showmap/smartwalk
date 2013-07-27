using System.Linq;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Views.Common;
using System;

namespace SmartWalk.iOS.Views.OrgEventView
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

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var venueShow = GetItemAt(indexPath) as VenueShow;

            if (venueShow != null &&
                _viewModel.ExpandCollapseShowCommand != null &&
                _viewModel.ExpandCollapseShowCommand.CanExecute(venueShow))
            {
                _viewModel.ExpandCollapseShowCommand.Execute(venueShow);
            }

            TableView.DeselectRow(indexPath, false);
        }

        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            return 76.0f;
        }

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);
            var venueShow = item as VenueShow;
            if (venueShow != null)
            {
                var height = VenueShowCell.CalculateCellHeight(
                    Equals(_viewModel.ExpandedShow, venueShow),
                    venueShow);

                return height;
            }


            throw new Exception("There is an unsupported type in the list.");
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

            headerView.PrepareForReuse();
            headerView.DataContext = VenueItemsSource[section];
            headerView.NavigateVenueCommand = _viewModel.NavigateVenueCommand;
            headerView.NavigateVenueOnMapCommand = _viewModel.NavigateVenueOnMapCommand;

            return headerView;
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            var cell = (VenueShowCell)tableView.DequeueReusableCell(VenueShowCell.Key, indexPath);
            cell.IsExpanded = Equals(_viewModel.ExpandedShow, item);
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
using System;
using System.Collections;
using System.Linq;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using System.Drawing;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public class OrgEventTableSource : MvxTableViewSource
    {
        private static readonly NSString EmptyCellKey = new NSString("empty");

        private readonly OrgEventViewModel _viewModel;

        private VenueShow[] _showItemsSource;

        private bool _isSearchBarMoved;

        public OrgEventTableSource(UITableView tableView, OrgEventViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;

            UseAnimations = true;

            tableView.RegisterClassForCellReuse(typeof(UITableViewCell), EmptyCellKey);
            tableView.RegisterNibForCellReuse(VenueShowCell.Nib, VenueShowCell.Key);
            tableView.RegisterNibForHeaderFooterViewReuse(VenueCell.Nib, VenueCell.Key);
        }

        public bool IsSearchSource { get; set; }

        public Venue[] VenueItemsSource
        {
            get { return ItemsSource != null ? (Venue[])ItemsSource : null; }
        }

        public VenueShow[] ShowItemsSource
        {
            get
            {
                if (_showItemsSource == null &&
                    VenueItemsSource != null)
                {
                    _showItemsSource = 
                        VenueItemsSource
                            .SelectMany(v => v.Shows)
                            .OrderBy(show => show.Start)
                            .ToArray();
                }

                return _showItemsSource;
            }
        }

        public override IEnumerable ItemsSource
        {
            set
            {
                _showItemsSource = null;
                base.ItemsSource = value;
            }
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (_isSearchBarMoved) return;

            var lastIndexPath = tableView.IndexPathsForVisibleRows.LastOrDefault();
            if (lastIndexPath != null &&
                indexPath.Section == lastIndexPath.Section &&
                indexPath.Row == lastIndexPath.Row)
            {
                _isSearchBarMoved = true;
                tableView.SetContentOffset(
                    new PointF(0, tableView.TableHeaderView.Frame.Height), false);
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);
            var venueShow = item as VenueShow;

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
            return _viewModel.IsGroupedByLocation ? 80f : 0;
        }

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);
            var venueShow = item as VenueShow;
            if (venueShow != null)
            {
                var height = VenueShowCell.CalculateCellHeight(
                    tableView.Frame.Width,
                    Equals(_viewModel.ExpandedShow, venueShow),
                    venueShow);

                return height;
            }

            return 35f;
        }

        public override int NumberOfSections(UITableView tableView)
        {
            return _viewModel.IsGroupedByLocation
                ? (VenueItemsSource != null ? VenueItemsSource.Count() : 0)
                : 1;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            if (_viewModel.IsGroupedByLocation)
            {
                var emptyRow = IsSearchSource &&
                    section == NumberOfSections(tableview) - 1 ? 1 : 0; // empty row for search

                return (VenueItemsSource != null && 
                        VenueItemsSource[section].Shows != null 
                    ? VenueItemsSource[section].Shows.Count() 
                    : 0) + emptyRow;
            }
            else
            {
                var showsCount = ShowItemsSource != null 
                    ? ShowItemsSource.Count()
                    : 0;

                return showsCount;
            }
        }

        public override UIView GetViewForHeader(UITableView tableView, int section)
        {
            if (_viewModel.IsGroupedByLocation)
            {
                var headerView = (VenueCell)tableView.DequeueReusableHeaderFooterView(VenueCell.Key);

                headerView.DataContext = VenueItemsSource[section];
                headerView.NavigateVenueCommand = _viewModel.NavigateVenueCommand;
                headerView.NavigateVenueOnMapCommand = _viewModel.NavigateVenueOnMapCommand;

                return headerView;
            }

            return null;
        }

        protected override UITableViewCell GetOrCreateCellFor(
            UITableView tableView,
            NSIndexPath indexPath,
            object item)
        {
            if (IsSearchSource && item == null)
            {
                var emptyCell = tableView.DequeueReusableCell(EmptyCellKey, indexPath);
                emptyCell.SelectionStyle = UITableViewCellSelectionStyle.None;
                return emptyCell;
            }

            var cell = (VenueShowCell)tableView.DequeueReusableCell(VenueShowCell.Key, indexPath);
            cell.DataContext = (VenueShow)item;
            cell.IsExpanded = Equals(_viewModel.ExpandedShow, item);
            return cell;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            if (_viewModel.IsGroupedByLocation)
            {
                if (VenueItemsSource != null &&
                    VenueItemsSource[indexPath.Section].Shows != null)
                {
                    // Asumming that there may be an empty row for search
                    return indexPath.Row < VenueItemsSource[indexPath.Section].Shows.Length
                        ? VenueItemsSource[indexPath.Section].Shows.ElementAt(indexPath.Row) 
                    : null;
                }
           
                return null;
            }
            else
            {
                if (ShowItemsSource != null)
                {
                    return ShowItemsSource[indexPath.Row];
                }

                return null;
            }
        }
    }
}
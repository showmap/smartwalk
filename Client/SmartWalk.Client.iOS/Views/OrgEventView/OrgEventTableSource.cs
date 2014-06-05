using System;
using System.Collections;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventTableSource : HiddenHeaderTableSource<Venue>
    {
        private static readonly NSString EmptyCellKey = new NSString("empty");

        private readonly OrgEventViewModel _viewModel;

        public OrgEventTableSource(OrgEventViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool IsSearchSource { get; set; }

        public NSIndexPath GetItemIndex(Show show)
        {
            for (var i = 0; i < ItemsSource.Length; i++)
            {
                if (ItemsSource[i].Shows.Contains(show))
                {
                    return NSIndexPath.FromItemSection(
                        Array.IndexOf(ItemsSource[i].Shows, show), 
                        i);
                }
            }

            return NSIndexPath.FromItemSection(0, 0);
        }

        public NSIndexPath GetItemIndex(Venue venue)
        {
            var venueNumber = Array.IndexOf(ItemsSource, venue);
            return venueNumber >= 0 
                ? NSIndexPath.FromRowSection(int.MaxValue, venueNumber)
                : null;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, false);
        }

        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            return _viewModel.IsGroupedByLocation || 
                (_viewModel.ExpandedShow != null && 
                ItemsSource[section].Shows.Contains(_viewModel.ExpandedShow)) 
                    ? VenueHeaderView.DefaultHeight : 0;
        }

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);
            var show = item as Show;
            if (show != null && show.Id == DayHeaderShow.Id)
            {
                return DayHeaderCell.DefaultHeight;
            }

            if (show != null)
            {
                var height = 
                    VenueShowCell.CalculateCellHeight(
                        tableView.Frame.Width,
                        Equals(_viewModel.ExpandedShow, show),
                        show);
                return height;
            }

            return VenueShowCell.DefaultHeight;
        }

        public override int NumberOfSections(UITableView tableView)
        {
            return ItemsSource != null ? ItemsSource.Length : 0;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            var emptyRow = IsSearchSource &&
                section == NumberOfSections(tableview) - 1 ? 1 : 0; // empty row for search

            return 
                (ItemsSource != null &&
                    ItemsSource[section].Shows != null 
                        ? ItemsSource[section].Shows.Length 
                        : 0) + emptyRow;
        }

        public override string TitleForHeader(UITableView tableView, int section)
        {
            return null;
        }

        public override UIView GetViewForHeader(UITableView tableView, int section)
        {
            if (_viewModel.IsGroupedByLocation || 
                (_viewModel.ExpandedShow != null && 
                    ItemsSource[section].Shows.Contains(_viewModel.ExpandedShow)))
            {
                var headerView = (VenueHeaderView)tableView.DequeueReusableHeaderFooterView(VenueHeaderView.Key);

                headerView.DataContext = ItemsSource[section];
                headerView.NavigateVenueCommand = _viewModel.NavigateVenueCommand;
                headerView.NavigateVenueOnMapCommand = _viewModel.NavigateVenueOnMapCommand;

                return headerView;
            }

            return null;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);
            var cell = GetOrCreateCellFor(tableView, indexPath, item);
            return cell;
        }

        protected override void OnTableViewReset(UITableView previousTableView, UITableView tableView)
        {
            if (tableView != null)
            {
                tableView.RegisterClassForHeaderFooterViewReuse(typeof(VenueHeaderView), VenueHeaderView.Key);
                tableView.RegisterClassForCellReuse(typeof(UITableViewCell), EmptyCellKey);
                tableView.RegisterClassForCellReuse(typeof(DayHeaderCell), DayHeaderCell.Key);
                tableView.RegisterNibForCellReuse(VenueShowCell.Nib, VenueShowCell.Key);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private UITableViewCell GetOrCreateCellFor(
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

            var cell = default(UITableViewCell);

            var show = item as Show;
            if (show != null && show.Id == DayHeaderShow.Id)
            {
                cell = tableView.DequeueReusableCell(DayHeaderCell.Key, indexPath);
                ((DayHeaderCell)cell).DataContext = show;
            }
            else if (show != null)
            {
                cell = tableView.DequeueReusableCell(VenueShowCell.Key, indexPath);
                ((VenueShowCell)cell).ShowImageFullscreenCommand = _viewModel.ShowHideFullscreenImageCommand;
                ((VenueShowCell)cell).ExpandCollapseShowCommand = _viewModel.ExpandCollapseShowCommand;
                ((VenueShowCell)cell).NavigateDetailsLinkCommand = _viewModel.NavigateWebLinkCommand;
                ((VenueShowCell)cell).DataContext = show;
                ((VenueShowCell)cell).IsExpanded = Equals(_viewModel.ExpandedShow, item);
                ((VenueShowCell)cell).IsHighlighted = ((VenueShowCell)cell).IsExpanded &&
                    !_viewModel.IsGroupedByLocation;
                ((VenueShowCell)cell).IsSeparatorVisible = 
                    !_viewModel.IsGroupedByLocation ||
                    indexPath.Row < ItemsSource[indexPath.Section].Shows.Length - 1 ||
                    indexPath.Section == ItemsSource.Length - 1;
            }

            return cell;
        }

        private object GetItemAt(NSIndexPath indexPath)
        {
            if (ItemsSource != null &&
                ItemsSource[indexPath.Section].Shows != null)
            {
                // Asumming that there may be an empty row for search
                return indexPath.Row < ItemsSource[indexPath.Section].Shows.Length
                    ? ItemsSource[indexPath.Section].Shows[indexPath.Row] 
                    : null;
            }
           
            return null;
        }
    }

    /// <summary>
    /// A helper base class that incapsulates the HACK for initial hiding of table's header view.
    /// </summary>
    public abstract class HiddenHeaderTableSource<T> : UITableViewSource, IListViewSource
    {
        private UITableView _tableView;
        private T[] _itemsSource;
        private bool _isTouched;

        protected HiddenHeaderTableSource()
        {
            IsAutohidingEnabled = true;
        }

        public bool IsAutohidingEnabled { get; set; }

        public UITableView TableView
        {
            get
            {
                return _tableView;
            }
            set
            {
                if (_tableView != value)
                {
                    var previousTable = _tableView;
                    _tableView = value;
                    OnTableViewReset(previousTable, _tableView);
                }
            }
        }

        public T[] ItemsSource
        {
            get
            {
                return _itemsSource;
            }
            set
            {
                if (!_itemsSource.EnumerableEquals(value))
                {
                    _itemsSource = value;
                    ReloadTableData();
                }
            }
        }

        IEnumerable IListViewSource.ItemsSource
        {
            get { return ItemsSource; }
        }

        public bool IsHeaderViewHidden
        {
            get
            {
                return TableView.TableHeaderView == null ||
                    TableView.ContentOffset.Y >= HeaderHeight;
            }
        }

        protected virtual float HeaderHeight
        {
            get
            {
                return TableView != null && 
                        TableView.TableHeaderView != null
                    ? TableView.TableHeaderView.Frame.Height 
                    : 0; 
            }
        }

        public void ReloadTableData()
        {
            TableView.ReloadData();

            if (IsAutohidingEnabled && !IsHeaderViewHidden)
            {
                ScrollUtil.ScrollOutHeaderAfterReload(
                    TableView, 
                    HeaderHeight, 
                    this, 
                    _isTouched);
            }
        }

        public void ScrollOutHeader()
        {
            if (IsAutohidingEnabled && !IsHeaderViewHidden)
            {
                ScrollUtil.ScrollOutHeader(
                    TableView, 
                    HeaderHeight, 
                    _isTouched);
            }
        }

        public override void DraggingStarted(UIScrollView scrollView)
        {
            _isTouched = true;
        }

        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            if (((UITableView)scrollView).TableHeaderView == null) return;

            ScrollUtil.AdjustHeaderPosition(scrollView, HeaderHeight);
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return 0;
        }

        protected virtual void OnTableViewReset(UITableView previousTableView, UITableView tableView)
        {
        }
    }
}
using System;
using System.Collections;
using System.Linq;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.GroupHeader;
using SmartWalk.Client.iOS.Resources;

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

        private bool ShowVenueGroupHeader
        {
            get
            {
                return
                    !_viewModel.IsGroupedByLocation &&
                    _viewModel.IsMultiday &&
                    _viewModel.SortBy == SortBy.Time &&
                    !_viewModel.CurrentDay.HasValue;
            }
        }

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
            var venueNumber = ItemsSource.ToList().FindIndex(v => v.Info.Id == venue.Info.Id);
            return venueNumber >= 0 
                ? NSIndexPath.FromRowSection(nint.MaxValue, venueNumber)
                : null;
        }

        public bool GetIsCellHeaderVisibile()
        {
            return !_viewModel.IsGroupedByLocation;
        }

        public bool GetIsCellSubHeaderVisibile(Show show = null)
        {
            return !_viewModel.IsGroupedByLocation &&
                _viewModel.SortBy == SortBy.Name && _viewModel.IsMultiday &&
                !_viewModel.CurrentDay.HasValue &&
                (show == null || show.StartTime.HasValue);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, false);
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            if (_viewModel.IsGroupedByLocation)
            {
                return VenueHeaderView.DefaultHeight;
            }

            var venue = ItemsSource[section];
            if (ShowVenueGroupHeader && venue.Info.Name != null)
            {
                return GroupHeaderView.DefaultHeight;
            }

            return 0;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var show = GetItemAt(indexPath);
            if (show != null && show.Id == Show.DayGroupId)
            {
                return DayHeaderCell.DefaultHeight;
            }

            if (show != null)
            {
                var isExpanded = Equals(_viewModel.ExpandedShow, show);
                var height = 
                    VenueShowCell.CalculateCellHeight(
                        tableView.Frame.Width,
                        isExpanded,
                        isExpanded && GetIsCellHeaderVisibile(),
                        isExpanded && GetIsCellSubHeaderVisibile(show),
                        show);
                return height;
            }

            return VenueShowCell.DefaultHeight;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return ItemsSource != null ? ItemsSource.Length : 0;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            var emptyRow = IsSearchSource &&
                section == NumberOfSections(tableview) - 1 ? 1 : 0; // empty row for search

            return 
                (ItemsSource != null &&
                    ItemsSource[section].Shows != null 
                        ? ItemsSource[section].Shows.Length 
                        : 0) + emptyRow;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return null;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var venue = ItemsSource[section];

            if (_viewModel.IsGroupedByLocation)
            {
                var headerView = (VenueHeaderView)tableView.DequeueReusableHeaderFooterView(VenueHeaderView.Key);

                headerView.DataContext = venue;
                headerView.NavigateVenueCommand = _viewModel.NavigateVenueCommand;
                headerView.NavigateVenueOnMapCommand = _viewModel.NavigateVenueOnMapCommand;

                return headerView;
            }

            if (ShowVenueGroupHeader && venue.Info.Name != null)
            {
                var groupView = (GroupHeaderView)tableView.DequeueReusableHeaderFooterView(GroupHeaderView.Key);

                groupView.DataContext = venue.Info.Name;

                return groupView;
            }

            return null;
        }

        public UIView GetHeaderForShowCell(bool isCellExpanded, Show show)
        {
            var headerView = isCellExpanded && GetIsCellHeaderVisibile()
                ? VenueHeaderContentView.Create()
                : null;

            if (headerView != null)
            {
                headerView.BackgroundColor = ThemeColors.PanelBackgroundAlpha;
                headerView.BackgroundView = headerView;

                headerView.DataContext = _viewModel.OrgEvent.Venues.GetVenueByShow(show);
                headerView.NavigateVenueCommand = _viewModel.NavigateVenueCommand;
                headerView.NavigateVenueOnMapCommand = _viewModel.NavigateVenueOnMapCommand;
            }

            return headerView;
        }

        public UIView GetSubHeaderForShowCell(bool isCellExpanded, Show show)
        {
            var subHeaderView = isCellExpanded && GetIsCellSubHeaderVisibile(show)
                ? GroupHeaderContentView.Create()
                : null;

            if (subHeaderView != null)
            {
                subHeaderView.BackgroundColor = ThemeColors.ContentLightBackgroundAlpha;

                subHeaderView.DataContext = show.StartTime.GetCurrentDayString();
            }

            return subHeaderView;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var show = GetItemAt(indexPath);

            if (IsSearchSource && show == null)
            {
                var emptyCell = tableView.DequeueReusableCell(EmptyCellKey, indexPath);
                emptyCell.SelectionStyle = UITableViewCellSelectionStyle.None;
                return emptyCell;
            }

            var cell = default(UITableViewCell);

            if (show != null && show.Id == Show.DayGroupId)
            {
                cell = tableView.DequeueReusableCell(DayHeaderCell.Key, indexPath);
                ((DayHeaderCell)cell).DataContext = show;
            }
            else if (show != null)
            {
                cell = tableView.DequeueReusableCell(VenueShowCell.Key, indexPath);
                var venueCell = (VenueShowCell)cell;
                venueCell.ShowImageFullscreenCommand = _viewModel.ShowHideFullscreenImageCommand;
                venueCell.ExpandCollapseShowCommand = _viewModel.ExpandCollapseShowCommand;
                venueCell.NavigateDetailsLinkCommand = _viewModel.NavigateWebLinkCommand;
                venueCell.DataContext = show;

                var isExpanded = Equals(_viewModel.ExpandedShow, show);
                venueCell.HeaderView = GetHeaderForShowCell(isExpanded, show);
                venueCell.SubHeaderView = GetSubHeaderForShowCell(isExpanded, show);
                venueCell.IsExpanded = isExpanded;

                venueCell.IsSeparatorVisible =
                    !IsLastInDayGroup(show, indexPath) && 
                    (IsInLastSection(indexPath) || !IsLastInSection(indexPath));
            }

            return cell;
        }

        protected override void OnTableViewReset(UITableView previousTableView, UITableView tableView)
        {
            if (tableView != null)
            {
                tableView.RegisterClassForHeaderFooterViewReuse(typeof(VenueHeaderView), VenueHeaderView.Key);
                tableView.RegisterClassForHeaderFooterViewReuse(typeof(GroupHeaderView), GroupHeaderView.Key);
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

        private Show GetItemAt(NSIndexPath indexPath)
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

        private bool IsLastInDayGroup(Show show, NSIndexPath indexPath)
        {
            var shows = ItemsSource[indexPath.Section].Shows;
            var index = Array.IndexOf(shows, show);
            var result = index < shows.Length - 1 && shows[index + 1].Id == Show.DayGroupId;
            return result;
        }

        private bool IsLastInSection(NSIndexPath indexPath)
        {
            var result = indexPath.Row == ItemsSource[indexPath.Section].Shows.Length - 1;
            return result;
        }

        private bool IsInLastSection(NSIndexPath indexPath)
        {
            var result = indexPath.Section == ItemsSource.Length - 1;
            return result;
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
                    TableView.ActualContentOffset() >= HeaderHeight;
            }
        }

        protected virtual float HeaderHeight
        {
            get
            {
                return TableView != null && 
                        TableView.TableHeaderView != null
                    ? (float)TableView.TableHeaderView.Frame.Height 
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
            if (IsAutohidingEnabled)
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

            ScrollUtil.AdjustHeaderPosition(scrollView, HeaderHeight, true);
        }

        public override void ScrolledToTop(UIScrollView scrollView)
        {
            ScrollOutHeader();
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 0;
        }

        protected virtual void OnTableViewReset(UITableView previousTableView, UITableView tableView)
        {
        }
    }
}
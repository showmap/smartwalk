using System;
using System.Collections;
using System.Linq;
using Foundation;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.GroupHeader;
using SmartWalk.Shared.Utils;
using UIKit;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventTableSource : HiddenHeaderTableSource<Venue>
    {
        private readonly OrgEventViewModel _viewModel;
        private readonly ScrollToHideUIManager _scrollToHideManager;

        public OrgEventTableSource(UITableView tableView, 
            OrgEventViewModel viewModel, 
            ScrollToHideUIManager scrollToHideManager = null) : base(tableView)
        {
            _viewModel = viewModel;
            _scrollToHideManager = scrollToHideManager;

            tableView.RegisterClassForHeaderFooterViewReuse(typeof(VenueHeaderView), VenueHeaderView.Key);
            tableView.RegisterClassForHeaderFooterViewReuse(typeof(GroupHeaderView), GroupHeaderView.Key);
            tableView.RegisterClassForCellReuse(typeof(DayHeaderCell), DayHeaderCell.Key);
            tableView.RegisterNibForCellReuse(VenueShowCell.Nib, VenueShowCell.Key);
        }

        private bool ShowDayGroupHeader
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

        private bool ShowTimeVisible
        {
            get
            {
                return 
                    !_viewModel.IsMultiday ||
                    _viewModel.IsGroupedByLocation ||
                    _viewModel.SortBy == SortBy.Time ||
                    _viewModel.CurrentDay.HasValue;
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

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, false);
        }

        public override void ReloadTableData()
        {
            base.ReloadTableData();

            if (_scrollToHideManager != null)
            {
                _scrollToHideManager.Reset();
            }
        }

        public override void DraggingStarted(UIScrollView scrollView)
        {
            base.DraggingStarted(scrollView);

            if (_scrollToHideManager != null)
            {   
                _scrollToHideManager.DraggingStarted();
            }
        }

        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            base.DraggingEnded(scrollView, willDecelerate);

            if (_scrollToHideManager != null)
            {
                _scrollToHideManager.DraggingEnded();
            }
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            if (_scrollToHideManager != null)
            {
                _scrollToHideManager.Scrolled();
            }
        }

        public override void ScrolledToTop(UIScrollView scrollView)
        {
            base.ScrolledToTop(scrollView);

            if (_scrollToHideManager != null)
            {
                _scrollToHideManager.ScrolledToTop();
            }
        }

        public override void DecelerationEnded(UIScrollView scrollView)
        {
            if (_scrollToHideManager != null)
            {
                _scrollToHideManager.ScrollFinished();
            }
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            if (_viewModel.IsGroupedByLocation)
            {
                return VenueHeaderView.DefaultHeight;
            }

            var venue = ItemsSource[section];
            if (ShowDayGroupHeader && venue.Info.Name != null)
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
                    VenueShowCell.CalculateCellHeight(tableView.Frame.Width, isExpanded, 
                        show, ShowTimeVisible, !_viewModel.IsGroupedByLocation);
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
            return 
                (ItemsSource != null &&
                    ItemsSource[section].Shows != null 
                        ? ItemsSource[section].Shows.Length 
                        : 0);
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

            if (ShowDayGroupHeader && venue.Info.Name != null)
            {
                var groupView = (GroupHeaderView)tableView.DequeueReusableHeaderFooterView(GroupHeaderView.Key);

                groupView.DataContext = venue.Info.Name;

                return groupView;
            }

            return null;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var show = GetItemAt(indexPath);
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
                venueCell.NavigateVenueOnMapCommand = _viewModel.NavigateVenueOnMapCommand;
                venueCell.NavigateDetailsLinkCommand = _viewModel.NavigateWebLinkCommand;
                venueCell.SetFavoriteCommand = _viewModel.FavoritesManager.SetFavoriteShowCommand;
                venueCell.UnsetFavoriteCommand = _viewModel.FavoritesManager.UnsetFavoriteShowCommand;
                venueCell.DataContext = new VenueShowDataContext(show, _viewModel.FavoritesManager, _viewModel.OrgEvent,
                    _viewModel.OrgEvent.ShowVenueShowLogos(),
                    _viewModel.IsGroupedByLocation, ShowTimeVisible);
                venueCell.IsExpanded = Equals(_viewModel.ExpandedShow, show);
                venueCell.IsBeforeExpanded = Equals(_viewModel.ExpandedShow, GetNextShow(show, indexPath));
                venueCell.IsSeparatorVisible =
                    !IsLastInDayGroup(show, indexPath) && 
                    (IsInLastSection(indexPath) || !IsLastInSection(indexPath));
            }

            return cell;
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

        private Show GetNextShow(Show show, NSIndexPath indexPath)
        {
            var shows = ItemsSource[indexPath.Section].Shows;
            var index = Array.IndexOf(shows, show);
            var result = index < shows.Length - 1 ? shows[index + 1] : null;
            return result;
        }
    }

    /// <summary>
    /// A helper base class that incapsulates the HACK for initial hiding of table's header view.
    /// </summary>
    public abstract class HiddenHeaderTableSource<T> : UITableViewSource, IListViewSource
    {
        private readonly UITableView _tableView;
        private T[] _itemsSource;
        private bool _isTouched;

        protected HiddenHeaderTableSource(UITableView tableView)
        {
            _tableView = tableView;
            IsAutohidingEnabled = true;
        }

        public bool IsAutohidingEnabled { get; set; }

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
                return _tableView.TableHeaderView == null ||
                    _tableView.ActualContentOffset() >= HeaderHeight;
            }
        }

        protected virtual float HeaderHeight
        {
            get
            {
                return _tableView != null && 
                    _tableView.TableHeaderView != null
                    ? (float)_tableView.TableHeaderView.Frame.Height 
                    : 0; 
            }
        }

        public virtual void ReloadTableData()
        {
            _tableView.ReloadData();

            if (IsAutohidingEnabled && !IsHeaderViewHidden)
            {
                ScrollUtil.ScrollOutHeaderAfterReload(
                    _tableView, 
                    HeaderHeight, 
                    this, 
                    _isTouched);
            }
        }

        public void ScrollOutHeader(bool animated)
        {
            if (IsAutohidingEnabled)
            {
                ScrollUtil.ScrollOutHeader(
                    _tableView, 
                    HeaderHeight, 
                    _isTouched && animated);
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
            ScrollOutHeader(true);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 0;
        }
    }

    public class OrgEventScrollToHideUIManager : ScrollToHideUIManager
    {
        private readonly UITableView _tableView;
        private readonly UIView _listSettingsView;

        public OrgEventScrollToHideUIManager(UITableView tableView, UIView listSettingsView) 
            : base(tableView, tableView.TableHeaderView.Frame.Height)
        {
            IsActive = true;
            _tableView = tableView;
            _listSettingsView = listSettingsView;
        }

        public bool IsActive { get; set; }

        protected override void OnHideUI()
        {
            if (IsActive)
            {
                _tableView.ContentInset = new UIEdgeInsets(
                    NavBarManager.NavBarHeight, 0, 0, 0);
                _listSettingsView.SetHidden(true, false);
            }
        }

        protected override void OnShowUI()
        {
            if (IsActive)
            { 
                _tableView.ContentInset = new UIEdgeInsets(
                    NavBarManager.NavBarHeight + ListSettingsView.DefaultHeight, 0, 0, 0);
                _listSettingsView.SetHidden(false, false);
            }
        }
    }
}
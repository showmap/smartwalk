using System;
using System.Collections;
using System.Linq;
using Cirrious.MvvmCross.Binding.Touch.Views;
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
    public class OrgEventTableSource : HiddenHeaderTableSource
    {
        private static readonly NSString EmptyCellKey = new NSString("empty");

        private readonly OrgEventViewModel _viewModel;

        public OrgEventTableSource(UITableView tableView, OrgEventViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;

            tableView.RegisterClassForHeaderFooterViewReuse(typeof(VenueHeaderView), VenueHeaderView.Key);
            tableView.RegisterClassForCellReuse(typeof(UITableViewCell), EmptyCellKey);
            tableView.RegisterClassForCellReuse(typeof(DayHeaderCell), DayHeaderCell.Key);
            tableView.RegisterNibForCellReuse(VenueShowCell.Nib, VenueShowCell.Key);
        }

        public bool IsSearchSource { get; set; }

        public new Venue[] ItemsSource
        {
            get
            {
                return (Venue[])base.ItemsSource;
            }
            set
            {
                if (!((Venue[])base.ItemsSource).EnumerableEquals(value))
                {
                    base.ItemsSource = value;
                    ReloadTableData();
                }
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
            var venueNumber = Array.IndexOf(ItemsSource, venue);
            return venueNumber >= 0 
                ? NSIndexPath.FromRowSection(int.MaxValue, venueNumber)
                : null;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            TableView.DeselectRow(indexPath, false);
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
            if (show != null && show.Id == DayBlankShow.Id)
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

            var cell = default(UITableViewCell);

            var show = item as Show;
            if (show != null && show.Id == DayBlankShow.Id)
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

        protected override object GetItemAt(NSIndexPath indexPath)
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }

    /// <summary>
    /// A helper base class that incapsulates the HACK for initial hiding of table's header view.
    /// </summary>
    public abstract class HiddenHeaderTableSource : MvxBaseTableViewSource, IListViewSource
    {
        private bool _isTouched;

        protected HiddenHeaderTableSource(UITableView tableView) : base(tableView)
        {
            IsAutohidingEnabled = true;
        }

        public bool IsAutohidingEnabled { get; set; }
        public IEnumerable ItemsSource { get; protected set; }

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

        public override void ReloadTableData()
        {
            base.ReloadTableData();

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
            if (TableView.TableHeaderView == null) return;

            ScrollUtil.AdjustHeaderPosition(scrollView, HeaderHeight);
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return 0;
        }

        protected override UITableViewCell GetOrCreateCellFor(
            UITableView tableView,
            NSIndexPath indexPath,
            object item)
        {
            return null;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            return null;
        }
    }
}
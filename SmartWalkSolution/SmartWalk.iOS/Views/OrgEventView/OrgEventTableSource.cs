using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using SmartWalk.Core.Utils;

namespace SmartWalk.iOS.Views.OrgEventView
{
    public class OrgEventTableSource : HiddenHeaderTableSource
    {
        private static readonly NSString EmptyCellKey = new NSString("empty");

        private readonly OrgEventViewModel _viewModel;

        private VenueShow[] _flattenItemsSource;

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


        public VenueShow[] FlattenItemsSource
        {
            get
            {
                if (_flattenItemsSource == null &&
                    VenueItemsSource != null)
                {
                    _flattenItemsSource = 
                        VenueItemsSource
                            .SelectMany(v => v.Shows)
                            .OrderBy(show => show.Start)
                            .ToArray();
                }

                return _flattenItemsSource;
            }
        }

        public override IEnumerable ItemsSource
        {
            set
            {
                _flattenItemsSource = null;
                base.ItemsSource = value;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
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
                var showsCount = FlattenItemsSource != null 
                    ? FlattenItemsSource.Count()
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
            cell.ShowImageFullscreenCommand = _viewModel.ShowHideFullscreenImageCommand;
            cell.ExpandCollapseShowCommand = _viewModel.ExpandCollapseShowCommand;
            cell.NavigateDetailsLinkCommand = _viewModel.NavigateWebLinkCommand;
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
                if (FlattenItemsSource != null)
                {
                    return FlattenItemsSource[indexPath.Row];
                }

                return null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }

    /// <summary>
    /// This is a helper base class that incapsulates the HACK for initial hiding of table's header view.
    /// </summary>
    public class HiddenHeaderTableSource : MvxTableViewSource
    {
        private bool _isTouched;
        private NSTimer _timer;

        protected HiddenHeaderTableSource(UITableView tableView) : base(tableView)
        {
        }

        public override void ReloadTableData()
        {
            base.ReloadTableData();

            if (ItemsSource != null && 
                ItemsSource.Cast<object>().Any() && 
                _timer == null)
            {
                _timer = NSTimer.CreateRepeatingScheduledTimer(
                    TimeSpan.MinValue, 
                    new NSAction(() => 
                    {
                        if (TableView.TableHeaderView != null &&
                            TableView.ContentSize.Height > TableView.TableHeaderView.Frame.Height)
                        {
                            TableView.SetContentOffset(
                                new PointF(0, TableView.TableHeaderView.Frame.Height), _isTouched);
                            _timer.Invalidate();
                            _timer.Dispose();
                            _timer = null;
                        }
                    }));
            }
        }

        public override void DraggingStarted(UIScrollView scrollView)
        {
            _isTouched = true;
        }

        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            if (TableView.TableHeaderView == null) return;

            if (TableView.ContentOffset.Y < 0 || scrollView.Decelerating) return;

            if (TableView.ContentOffset.Y < TableView.TableHeaderView.Frame.Height / 2)
            {
                TableView.SetContentOffset(new PointF(0, 0), true);
            }
            else if (TableView.ContentOffset.Y < TableView.TableHeaderView.Frame.Height)
            {
                TableView.SetContentOffset(
                    new PointF(0, TableView.TableHeaderView.Frame.Height), true);
            }
        }

        protected override UITableViewCell GetOrCreateCellFor(
            UITableView tableView,
            NSIndexPath indexPath,
            object item)
        {
            return null;
        }
    }
}
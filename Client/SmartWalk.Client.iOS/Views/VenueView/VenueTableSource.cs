using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.EntityCell;
using SmartWalk.Client.iOS.Views.Common.GroupHeader;
using SmartWalk.Client.iOS.Views.OrgEventView;

namespace SmartWalk.Client.iOS.Views.VenueView
{
    public class VenueTableSource : MvxTableViewSource, IListViewSource
    {
        private readonly VenueViewModel _viewModel;
        private readonly ScrollToHideUIManager _scrollToHideManager;

        public VenueTableSource(UITableView tableView, VenueViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;

            tableView.RegisterClassForHeaderFooterViewReuse(typeof(GroupHeaderView), GroupHeaderView.Key);
            tableView.RegisterNibForCellReuse(EntityCell.Nib, EntityCell.Key);
            tableView.RegisterNibForCellReuse(VenueShowCell.Nib, VenueShowCell.Key);
            tableView.RegisterNibForCellReuse(NextVenueCell.Nib, NextVenueCell.Key);

            _scrollToHideManager = new ScrollToHideUIManager(tableView);
        }

        public GroupContainer[] GroupItemsSource
        {
            get { return (GroupContainer[])ItemsSource;}
        }

        public override void ReloadTableData()
        {
            base.ReloadTableData();

            _scrollToHideManager.Reset();
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            TableView.DeselectRow(indexPath, false);
        }

        public override void DraggingStarted(UIScrollView scrollView)
        {
            _scrollToHideManager.DraggingStarted();
        }

        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            _scrollToHideManager.DraggingEnded();
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            _scrollToHideManager.Scrolled();
        }

        public override void ScrolledToTop(UIScrollView scrollView)
        {
            _scrollToHideManager.ScrolledToTop();
        }

        public override void DecelerationEnded(UIScrollView scrollView)
        {
            _scrollToHideManager.ScrollFinished();
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 
                GroupItemsSource != null &&
                GroupItemsSource[section].Key != null 
                ? GroupHeaderView.DefaultHeight 
                    : 0f;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);

            var entityCellContext = item as IEntityCellContext;
            if (entityCellContext != null)
            {
                var height = EntityCell.CalculateCellHeight(
                    UIApplication.SharedApplication.KeyWindow.Bounds.Size,
                    entityCellContext);
                return height;
            }

            var venueShow = item as Show;
            if (venueShow != null)
            {
                var height = VenueShowCell.CalculateCellHeight(
                    tableView.Frame.Width,
                    Equals(_viewModel.ExpandedShow, venueShow),
                    venueShow);
                return height;
            }

            if (item is string)
            {
                return NextVenueCell.DefaultHeight;
            }

            throw new Exception("There is an unsupported type in the list.");
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return GroupItemsSource != null ? GroupItemsSource.Length : 0;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return GroupItemsSource != null ? GroupItemsSource[section].Count : 0;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return null;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var dataContext = GroupItemsSource != null ? GroupItemsSource[section].Key : null;
            if (dataContext != null)
            {
                var headerView = (GroupHeaderView)tableView
                    .DequeueReusableHeaderFooterView(GroupHeaderView.Key);

                headerView.DataContext = dataContext;

                return headerView;
            }

            return null;
        }

        protected override UITableViewCell GetOrCreateCellFor(
            UITableView tableView, 
            NSIndexPath indexPath, 
            object item)
        {
            var cell = default(UITableViewCell);

            var entityCellContext = item as IEntityCellContext;
            if (entityCellContext != null)
            {
                cell = tableView.DequeueReusableCell(EntityCell.Key, indexPath);
                ((EntityCell)cell).ExpandCollapseCommand = _viewModel.ExpandCollapseCommand;
                ((EntityCell)cell).ShowImageFullscreenCommand = _viewModel.ShowHideFullscreenImageCommand;
                ((EntityCell)cell).NavigateWebSiteCommand = _viewModel.NavigateWebLinkCommand;
                ((EntityCell)cell).NavigateAddressesCommand = _viewModel.NavigateAddressesCommand;
                ((EntityCell)cell).DataContext = entityCellContext;
            }

            var venueShow = item as Show;
            if (venueShow != null)
            {
                cell = tableView.DequeueReusableCell(VenueShowCell.Key, indexPath);
                ((VenueShowCell)cell).ShowImageFullscreenCommand = _viewModel.ShowHideFullscreenImageCommand;
                ((VenueShowCell)cell).ExpandCollapseShowCommand = _viewModel.ExpandCollapseShowCommand;
                ((VenueShowCell)cell).NavigateDetailsLinkCommand = _viewModel.NavigateWebLinkCommand;
                ((VenueShowCell)cell).DataContext = venueShow;
                ((VenueShowCell)cell).IsExpanded = Equals(_viewModel.ExpandedShow, item);
                ((VenueShowCell)cell).IsSeparatorVisible = 
                    indexPath.Row < GroupItemsSource[indexPath.Section].Count - 1 ||
                    indexPath.Section == GroupItemsSource.Length - 2; // assuming that last section is next-entity button
            }

            var nextVenueTitle = item as string;
            if (nextVenueTitle != null)
            {
                cell = tableView.DequeueReusableCell(NextVenueCell.Key, indexPath);
                ((NextVenueCell)cell).ShowNextEntityCommand = _viewModel.ShowNextEntityCommand;
                ((NextVenueCell)cell).DataContext = nextVenueTitle;
            }

            return cell;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            return indexPath.Row < GroupItemsSource[indexPath.Section].Count
                ? GroupItemsSource[indexPath.Section][indexPath.Row]
                : null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }
}
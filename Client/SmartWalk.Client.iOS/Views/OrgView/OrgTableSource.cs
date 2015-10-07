using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.Model;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.EntityCell;
using SmartWalk.Client.iOS.Views.Common.GroupHeader;

namespace SmartWalk.Client.iOS.Views.OrgView
{
    public class OrgTableSource : MvxTableViewSource, IListViewSource
    {
        private readonly OrgViewModel _viewModel;
        private readonly ScrollToHideUIManager _scrollToHideManager;

        public OrgTableSource(UITableView tableView, OrgViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;

            tableView.RegisterClassForHeaderFooterViewReuse(typeof(GroupHeaderView), GroupHeaderView.Key);
            tableView.RegisterNibForCellReuse(EntityCell.Nib, EntityCell.Key);
            tableView.RegisterNibForCellReuse(OrgEventCell.Nib, OrgEventCell.Key);

            _scrollToHideManager = new ScrollToHideUIManager(tableView);
        }

        public GroupContainer[] GroupItemsSource
        {
            get { return (GroupContainer[])ItemsSource; }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var eventInfo = GetItemAt(indexPath) as OrgEvent;

            if (eventInfo != null &&
                _viewModel.NavigateOrgEventViewCommand.CanExecute(eventInfo))
            {
                _viewModel.NavigateOrgEventViewCommand.Execute(eventInfo);
            }

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

            if (item is OrgEvent)
            {
                return OrgEventCell.DefaultHeight;
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
                ((EntityCell)cell).ShowHideModalViewCommand = _viewModel.ShowHideModalViewCommand;
                ((EntityCell)cell).NavigateAddressesCommand = _viewModel.NavigateAddressesCommand;
                ((EntityCell)cell).DataContext = entityCellContext;
            }

            var orgEvent = item as OrgEvent;
            if (orgEvent != null)
            {
                cell = tableView.DequeueReusableCell(OrgEventCell.Key, indexPath);
                ((OrgEventCell)cell).DataContext = orgEvent;
                ((OrgEventCell)cell).IsSeparatorVisible = true;
            }

            return cell;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            return GroupItemsSource[indexPath.Section][indexPath.Row];
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }
}
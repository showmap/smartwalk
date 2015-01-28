using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Foundation;
using UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.EntityCell;

namespace SmartWalk.Client.iOS.Views.OrgEventInfoView
{
    public class OrgEventInfoTableSource : MvxTableViewSource, IListViewSource
    {
        private readonly OrgEventInfoViewModel _viewModel;
        private readonly ScrollToHideUIManager _scrollToHideManager;

        public OrgEventInfoTableSource(UITableView tableView, OrgEventInfoViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;

            tableView.RegisterNibForCellReuse(EntityCell.Nib, EntityCell.Key);

            _scrollToHideManager = new ScrollToHideUIManager(tableView);
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

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);

            var entityCellContext = item as IEntityCellContext;
            if (entityCellContext != null)
            {
                var height = EntityCell.CalculateCellHeight(tableView.Frame.Size, entityCellContext);
                return height;
            }

            throw new Exception("There is an unsupported type in the list.");
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
                ((EntityCell)cell).NavigateAddressesCommand = _viewModel.NavigateAddressesCommand;
                ((EntityCell)cell).DataContext = entityCellContext;
            }

            return cell;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }
}
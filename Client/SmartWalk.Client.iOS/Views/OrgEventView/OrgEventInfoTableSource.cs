using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.EntityCell;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public class OrgEventInfoTableSource : MvxTableViewSource, IListViewSource
    {
        private readonly OrgEventInfoViewModel _viewModel;

        public OrgEventInfoTableSource(UITableView tableView, OrgEventInfoViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;

            tableView.RegisterNibForCellReuse(EntityCell.Nib, EntityCell.Key);
        }

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);

            var entityCellContext = item as IEntityCellContext;
            if (entityCellContext != null)
            {
                var height = EntityCell.CalculateCellHeight(
                    tableView.Frame.Width,
                    entityCellContext.IsDescriptionExpanded,
                    entityCellContext.Entity);

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
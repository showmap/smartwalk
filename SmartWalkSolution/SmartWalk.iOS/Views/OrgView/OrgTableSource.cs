using System;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Views.Common;
using SmartWalk.iOS.Views.Common.EntityCell;

namespace SmartWalk.iOS.Views.OrgView
{
    public class OrgTableSource : MvxTableViewSource
    {
        private readonly OrgViewModel _viewModel;

        public OrgTableSource(UITableView tableView, OrgViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;

            tableView.RegisterNibForHeaderFooterViewReuse(GroupHeaderCell.Nib, GroupHeaderCell.Key);
            tableView.RegisterNibForCellReuse(EntityCell.Nib, EntityCell.Key);
            tableView.RegisterNibForCellReuse(OrgEventCell.Nib, OrgEventCell.Key);
        }

        public ICommand ShowImageFullscreenCommand { get; set; }

        public GroupContainer[] GroupItemsSource
        {
            get { return (GroupContainer[])ItemsSource;}
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var eventInfo = GetItemAt(indexPath) as OrgEventInfo;

            if (eventInfo != null &&
                _viewModel.NavigateOrgEventViewCommand.CanExecute(eventInfo))
            {
                _viewModel.NavigateOrgEventViewCommand.Execute(eventInfo);
            }

            TableView.DeselectRow(indexPath, false);
        }

        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            return TitleForHeader(tableView, section) != null ? 23.0f : 0f;
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
                    entityCellContext.Entity,
                    EntityCell.DefaultImageHeight);

                return height;
            }

            if (item is OrgEventInfo)
            {
                return 50.0f;
            }

            throw new Exception("There is an unsupported type in the list.");
        }

        public override int NumberOfSections(UITableView tableView)
        {
            return GroupItemsSource != null ? GroupItemsSource.Count() : 0;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return GroupItemsSource != null ? GroupItemsSource[section].Count : 0;
        }

        public override string TitleForHeader(UITableView tableView, int section)
        {
            return GroupItemsSource != null ? GroupItemsSource[section].Key : null;
        }

        public override UIView GetViewForHeader(UITableView tableView, int section)
        {
            var title = TitleForHeader(tableView, section);

            if (title != null)
            {
                var headerView = (GroupHeaderCell)tableView.DequeueReusableHeaderFooterView(GroupHeaderCell.Key);

                headerView.Text = title;

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
                ((EntityCell)cell).ShowImageFullscreenCommand = ShowImageFullscreenCommand;
                ((EntityCell)cell).NavigateWebSiteCommand = _viewModel.NavigateWebLinkCommand;
                ((EntityCell)cell).IsLogoSizeFixed = true;
                ((EntityCell)cell).DataContext = entityCellContext;
            }

            var orgEventInfo = item as OrgEventInfo;
            if (orgEventInfo != null)
            {
                cell = tableView.DequeueReusableCell(OrgEventCell.Key, indexPath);
                ((OrgEventCell)cell).DataContext = orgEventInfo;
            }

            return cell;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            return GroupItemsSource[indexPath.Section][indexPath.Row];
        }
    }
}
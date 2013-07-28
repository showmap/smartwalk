using System;
using System.Linq;
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
        private readonly ViewsFactory<GroupHeaderCell> _headerViewFactory;

        public OrgTableSource(UITableView tableView, OrgViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;
            _headerViewFactory = new ViewsFactory<GroupHeaderCell>(GroupHeaderCell.Create);

            UseAnimations = true;

            tableView.RegisterNibForCellReuse(EntityCell.Nib, EntityCell.Key);
            tableView.RegisterNibForCellReuse(OrgEventCell.Nib, OrgEventCell.Key);
        }

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

            if (item is OrgViewModel)
            {
                var height = EntityCell.CalculateCellHeight(
                    _viewModel.IsDescriptionExpanded,
                    _viewModel.Org);

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
                var headerView = _headerViewFactory.DequeueReusableView();

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

            var orgViewModel = item as OrgViewModel;
            if (orgViewModel != null)
            {
                cell = tableView.DequeueReusableCell(EntityCell.Key, indexPath);
                ((EntityCell)cell).IsLogoSizeFixed = true;
                ((EntityCell)cell).DataContext = orgViewModel;
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
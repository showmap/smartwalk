using System;
using System.Linq;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Views.Common;
using SmartWalk.iOS.Views.OrgEventView;

namespace SmartWalk.iOS.Views.VenueView
{
    public class VenueTableSource : MvxTableViewSource
    {
        private readonly VenueViewModel _viewModel;
        private readonly ViewsFactory<GroupHeaderCell> _headerViewFactory;

        public VenueTableSource(UITableView tableView, VenueViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;
            _headerViewFactory = new ViewsFactory<GroupHeaderCell>(GroupHeaderCell.Create);

            UseAnimations = true;

            tableView.RegisterNibForCellReuse(EntityCell.Nib, EntityCell.Key);
            tableView.RegisterNibForCellReuse(VenueShowCell.Nib, VenueShowCell.Key);
        }

        public GroupContainer[] GroupItemsSource
        {
            get { return (GroupContainer[])ItemsSource;}
        }

        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            return TitleForHeader(tableView, section) != null ? 23.0f : 0f;
        }

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);

            if (item is VenueViewModel)
            {
                var height = EntityCell.CalculateCellHeight(
                    _viewModel.IsDescriptionExpanded,
                    _viewModel.Venue);

                return height;
            }

            if (item is VenueShow)
            {
                return 35.0f;
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
            var key = default(NSString);

            if (item is VenueViewModel)
            {
                key = EntityCell.Key;
            }

            if (item is VenueShow)
            {
                key = VenueShowCell.Key;
            }

            var cell = tableView.DequeueReusableCell(key, indexPath);
            return cell;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            return GroupItemsSource[indexPath.Section][indexPath.Row];
        }
    }
}
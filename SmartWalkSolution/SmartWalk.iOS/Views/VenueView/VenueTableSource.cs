using System;
using System.Linq;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Views.Common;
using SmartWalk.iOS.Views.Common.EntityCell;
using SmartWalk.iOS.Views.OrgEventView;

namespace SmartWalk.iOS.Views.VenueView
{
    public class VenueTableSource : MvxTableViewSource
    {
        private readonly VenueViewModel _viewModel;
        private readonly ViewsFactory<GroupHeaderCell> _headerViewFactory;

        private int _entityImageHeight = 0;

        public VenueTableSource(UITableView tableView, VenueViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;
            _headerViewFactory = new ViewsFactory<GroupHeaderCell>(GroupHeaderCell.Create);

            tableView.RegisterNibForCellReuse(EntityCell.Nib, EntityCell.Key);
            tableView.RegisterNibForCellReuse(VenueShowCell.Nib, VenueShowCell.Key);
        }

        public GroupContainer[] GroupItemsSource
        {
            get { return (GroupContainer[])ItemsSource;}
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var venueShow = GetItemAt(indexPath) as VenueShow;

            if (venueShow != null &&
                _viewModel.ExpandCollapseShowCommand != null &&
                _viewModel.ExpandCollapseShowCommand.CanExecute(venueShow))
            {
                _viewModel.ExpandCollapseShowCommand.Execute(venueShow);
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
                    _entityImageHeight);

                return height;
            }

            var venueShow = item as VenueShow;
            if (venueShow != null)
            {
                var height = VenueShowCell.CalculateCellHeight(
                    tableView.Frame.Width,
                    Equals(_viewModel.ExpandedShow, venueShow),
                    venueShow);

                return height;
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

        public override void ReloadTableData()
        {
            _entityImageHeight = 0;

            base.ReloadTableData();
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
                ((EntityCell)cell).ImageHeightUpdatedHandler = OnEntityImageHeightUpdated;
                ((EntityCell)cell).DataContext = entityCellContext;
            }

            var venueShow = item as VenueShow;
            if (venueShow != null)
            {
                cell = tableView.DequeueReusableCell(VenueShowCell.Key, indexPath);
                ((VenueShowCell)cell).DataContext = venueShow;
                ((VenueShowCell)cell).IsExpanded = Equals(_viewModel.ExpandedShow, item);
            }

            return cell;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            return GroupItemsSource[indexPath.Section][indexPath.Row];
        }

        private void OnEntityImageHeightUpdated(int imageHeight, bool updateTable)
        {
            _entityImageHeight = imageHeight;

            if (updateTable)
            {
                DispatchQueue.DefaultGlobalQueue.DispatchAsync(() => 
                    {
                        BeginInvokeOnMainThread(() => 
                            {
                                TableView.BeginUpdates();
                                TableView.EndUpdates();
                            });
                    });
            }
        }
    }
}
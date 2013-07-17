using System;
using System.ComponentModel;
using System.Linq;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Converters;
using SmartWalk.iOS.Views.Cells;
using SmartWalk.iOS.Utils;

namespace SmartWalk.iOS.Views
{
    public partial class VenueView : TableViewBase
    {
        public new VenueViewModel ViewModel
        {
            get { return (VenueViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override UITableView TableView { get { return VenueShowsTableView; } }

        protected override void UpdateViewTitle()
        {
            if (ViewModel.Venue != null && ViewModel.Venue.Info != null)
            {
                NavigationItem.Title = ViewModel.Venue.Info.Name;
            }
        }

        protected override MvxTableViewSource CreateTableViewSource()
        {
            var tableSource = new VenueAndShowsTableSource(VenueShowsTableView, ViewModel);

            this.CreateBinding(tableSource).To((VenueViewModel vm) => vm)
                .WithConversion(new VenueAndShowsTableSourceConverter(), null).Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                VenueShowsTableView.BeginUpdates();
                VenueShowsTableView.EndUpdates();
            }
        }
    }

    public class VenueAndShowsTableSource : MvxTableViewSource
    {
        private readonly VenueViewModel _viewModel;
        private readonly ViewsFactory<GroupHeaderCell> _headerViewFactory;

        public VenueAndShowsTableSource(UITableView tableView, VenueViewModel viewModel)
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

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
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
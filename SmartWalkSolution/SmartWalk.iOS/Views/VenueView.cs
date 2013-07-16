using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Converters;
using SmartWalk.iOS.Views.Cells;

namespace SmartWalk.iOS.Views
{
    public partial class VenueView : MvxViewController
    {
        private UIRefreshControl _refreshControl;

        public new VenueViewModel ViewModel
        {
            get { return (VenueViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            
            UpdateViewTitle();

            InitializeTableView();
        }

        private void UpdateViewTitle()
        {
            if (ViewModel.Venue != null && ViewModel.Venue.Info != null)
            {
                NavigationItem.Title = ViewModel.Venue.Info.Name;
            }
        }

        private void InitializeTableView()
        {
            var tableSource = new VenueAndShowsTableSource(VenueShowsTableView, ViewModel);

            this.CreateBinding(tableSource).To((VenueViewModel vm) => vm)
                .WithConversion(new VenueAndShowsTableSourceConverter(), null).Apply();

            VenueShowsTableView.Source = tableSource;
            VenueShowsTableView.ReloadData();

            _refreshControl = new UIRefreshControl();
            _refreshControl.ValueChanged += (sender, e) => 
                {
                    if (ViewModel.RefreshCommand.CanExecute(null))
                    {
                        ViewModel.RefreshCommand.Execute(null);
                    }
                };

            VenueShowsTableView.AddSubview(_refreshControl);
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.Venue))
            {
                UpdateViewTitle();
                InvokeOnMainThread(_refreshControl.EndRefreshing);
            }
            else if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                VenueShowsTableView.BeginUpdates();
                VenueShowsTableView.EndUpdates();
            }
        }
    }

    public class VenueAndShowsTableSource : MvxTableViewSource
    {
        private readonly VenueViewModel _viewModel;

        public VenueAndShowsTableSource(UITableView tableView, VenueViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;

            UseAnimations = true;

            tableView.RegisterNibForCellReuse(EntityCell.Nib, EntityCell.Key);
            tableView.RegisterNibForCellReuse(VenueShowCell.Nib, VenueShowCell.Key);
        }

        public GroupContainer[] GroupItemsSource
        {
            get { return (GroupContainer[])ItemsSource;}
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

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
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
using System;
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
using SmartWalk.iOS.Views.Cells;
using SmartWalk.iOS.Views.Converters;

namespace SmartWalk.iOS.Views
{
    public partial class OrgView : MvxViewController
    {
        public new OrgViewModel ViewModel
        {
            get { return (OrgViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var tableSource = new OrgAndEventsTableSource(OrgEventsTableView, ViewModel);

            this.CreateBinding(tableSource).To((OrgViewModel vm) => vm)
                .WithConversion(new OrgAndEventsTableSourceConverter(), null).Apply();

            OrgEventsTableView.Source = tableSource;
            OrgEventsTableView.ReloadData();

            var refreshControl = new UIRefreshControl();
            refreshControl.ValueChanged += (sender, e) => 
                {
                    if (ViewModel.RefreshCommand.CanExecute(null))
                    {
                        ViewModel.RefreshCommand.Execute(null);
                    }
                };

            ViewModel.PropertyChanged += (sender, e) => 
                {
                    if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.Org))
                    {
                        InvokeOnMainThread(refreshControl.EndRefreshing);
                    }
                    else if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
                    {
                        OrgEventsTableView.BeginUpdates();
                        OrgEventsTableView.EndUpdates();
                    }
                };

            OrgEventsTableView.AddSubview(refreshControl);

            NavigationItem.Title = ViewModel.Org.Info.Name;
        }
    }

    public class OrgAndEventsTableSource : MvxTableViewSource
    {
        private readonly OrgViewModel _viewModel;

        public OrgAndEventsTableSource(UITableView tableView, OrgViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;

            UseAnimations = true;

            tableView.RegisterNibForCellReuse(EntityCell.Nib, EntityCell.Key);
            tableView.RegisterNibForCellReuse(OrgEventCell.Nib, OrgEventCell.Key);
        }

        public GroupContainer[] GroupItemsSource
        {
            get { return ItemsSource as GroupContainer[];}
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

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);

            if (item is OrgViewModel)
            {
                if (_viewModel.IsDescriptionExpanded)
                {
                    var orgViewModel = (OrgViewModel)item;

                    var isVertical = 
                        UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.Portrait || 
                            UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.PortraitUpsideDown;

                    var frameSize = new SizeF(
                            (isVertical 
                               ? UIScreen.MainScreen.Bounds.Width 
                               : UIScreen.MainScreen.Bounds.Height),
                        float.MaxValue); // TODO:  - UIConstants.DefaultTextMargin * 2

                    var textSize = new NSString(orgViewModel.Org.Description).StringSize(
                        UIFont.FromName("Helvetica-Bold", 15),
                        frameSize,
                        UILineBreakMode.WordWrap);

                    return textSize.Height + 274f;
                }
                else
                {
                    return 350.0f;
                }
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

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {
            var key = default(NSString);

            if (item is OrgViewModel)
            {
                key = EntityCell.Key;
            }

            if (item is OrgEventInfo)
            {
                key = OrgEventCell.Key;
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
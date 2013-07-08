using System;
using System.ComponentModel;
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
using MonoTouch;
using System.Drawing;

namespace SmartWalk.iOS.Views
{
    public partial class OrgView : MvxViewController
    {
        public new OrgViewModel ViewModel
        {
            get { return (OrgViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var tableSource = new OrgEventTableSource(OrgEventsTableView, ViewModel);

            this.CreateBinding(tableSource).To((OrgViewModel vm) => vm.Org)
                .WithConversion(new OrgTableSourceConverter(), null).Apply();

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
                        InvokeOnMainThread(() => refreshControl.EndRefreshing());
                    }
                };

            OrgEventsTableView.AddSubview(refreshControl);

            NavigationItem.Title = ViewModel.Org.Info.Name;
        }
    }

    public class OrgEventTableSource : MvxTableViewSource
    {
        private OrgViewModel _viewModel;

        public OrgEventTableSource(UITableView tableView, OrgViewModel viewModel)
            : base(tableView)
        {
            _viewModel = viewModel;

            UseAnimations = true;
            AddAnimation = UITableViewRowAnimation.Top;
            RemoveAnimation = UITableViewRowAnimation.Middle;

            tableView.RegisterNibForCellReuse(OrgCell.Nib, OrgCell.Key);
            tableView.RegisterNibForCellReuse(OrgEventCell.Nib, OrgEventCell.Key);
        }

        public GroupContainer[] GroupItemsSource
        {
            get { return ItemsSource as GroupContainer[];}
        }

        public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
        {
            var item = GetItemAt(indexPath);

            if (item is Org)
            {
                if (_viewModel.IsDescriptionExpanded)
                {
                    var org = (Org)item;
                    var size = new NSString(org.Description).StringSize(
                        UIFont.FromName("Helvetica-Bold", 15),
                        new SizeF(320, float.MaxValue),
                        UILineBreakMode.WordWrap);

                    return size.Height + 294f;
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
            return GroupItemsSource.Count();
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return GroupItemsSource[section].Count;
        }

        public override string TitleForHeader(UITableView tableView, int section)
        {
            return GroupItemsSource[section].Key;
        }

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {
            var key = default(NSString);

            if (item is Org)
            {
                key = OrgCell.Key;
            }

            if (item is OrgEventInfo)
            {
                key = OrgEventCell.Key;
            }

            var cell = tableView.DequeueReusableCell(key, indexPath);

            var orgCell = cell as OrgCell;
            if (orgCell != null)
            {
                orgCell.ExpandCollapseClick += (sender, e) => 
                    {
                        _viewModel.IsDescriptionExpanded = !_viewModel.IsDescriptionExpanded;

                        TableView.BeginUpdates();
                        TableView.EndUpdates();
                    };
            }

            return cell;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            return GroupItemsSource[indexPath.Section][indexPath.Row];
        }
    }

    /*public class LogoImageConverter : IMvxValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return UIImage.FromFile((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }*/
}
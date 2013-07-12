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

namespace SmartWalk.iOS.Views
{
    public partial class OrgEventView : MvxViewController
    {
        public new OrgEventViewModel ViewModel
        {
            get { return (OrgEventViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            var tableSource = new VenuesAndShowsTableSource(VenuesAndShowsTableView);

            this.CreateBinding(tableSource).To((OrgEventViewModel vm) => vm.OrgEvent.Venues).Apply();

            VenuesAndShowsTableView.Source = tableSource;
            VenuesAndShowsTableView.ReloadData();

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
                if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.OrgEvent))
                {
                    InvokeOnMainThread(refreshControl.EndRefreshing);
                }
            };

            VenuesAndShowsTableView.AddSubview(refreshControl);
        }
    }

    public class VenuesAndShowsTableSource : MvxTableViewSource
    {
        public VenuesAndShowsTableSource(UITableView tableView)
            : base(tableView)
        {
            UseAnimations = true;

            tableView.RegisterNibForCellReuse(VenueCell.Nib, VenueCell.Key);
            tableView.RegisterNibForCellReuse(VenueShowCell.Nib, VenueShowCell.Key);
        }

        public Venue[] VenueItemsSource
        {
            get { return ItemsSource != null ? (Venue[])ItemsSource : null; }
        }

        public override float GetHeightForHeader(UITableView tableView, int section)
        {
            return 40.0f;
        }

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 35.0f;
        }

        public override int NumberOfSections(UITableView tableView)
        {
            return VenueItemsSource != null ? VenueItemsSource.Count() : 0;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return VenueItemsSource != null && VenueItemsSource[section].Shows != null 
                ? VenueItemsSource[section].Shows.Count() 
                : 0;
        }

        public override UIView GetViewForHeader(UITableView tableView, int section)
        {
            var cell = (VenueCell)tableView.DequeueReusableCell(VenueCell.Key);
            cell.DataContext = VenueItemsSource[section];
            return cell;
        }

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {
            var cell = tableView.DequeueReusableCell(VenueShowCell.Key, indexPath);
            return cell;
        }

        protected override object GetItemAt(NSIndexPath indexPath)
        {
            return VenueItemsSource != null && VenueItemsSource[indexPath.Section].Shows != null 
                ? VenueItemsSource[indexPath.Section].Shows.ElementAt(indexPath.Row) 
                : null;
        }
    }
}
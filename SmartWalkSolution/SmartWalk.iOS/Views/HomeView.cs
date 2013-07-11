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
    public partial class HomeView : MvxViewController
    {
        public new HomeViewModel ViewModel
        {
            get { return (HomeViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var tableSource = new OrgTableSource(OrgTableView, ViewModel);

            this.CreateBinding(tableSource).To((HomeViewModel vm) => vm.OrgInfos).Apply();

            OrgTableView.Source = tableSource;
            OrgTableView.ReloadData();

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
                    if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.OrgInfos))
                    {
                        InvokeOnMainThread(refreshControl.EndRefreshing);
                    }
                };
                     
            OrgTableView.AddSubview(refreshControl);
        }
    }

    public class OrgTableSource : MvxTableViewSource
    {
        private HomeViewModel _viewModel;

        public OrgTableSource(UITableView tableView, HomeViewModel homeViewModel)
            : base(tableView)
        {
            _viewModel = homeViewModel;

            UseAnimations = true;

            tableView.RegisterNibForCellReuse(OrgInfoCell.Nib, OrgInfoCell.Key);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var org = (EntityInfo)GetItemAt(indexPath);

            if (_viewModel.NavigateOrgViewCommand.CanExecute(org))
            {
                _viewModel.NavigateOrgViewCommand.Execute(org);
            }

            TableView.DeselectRow(indexPath, false);
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            return tableView.DequeueReusableCell(OrgInfoCell.Key, indexPath);
        }
    }
}
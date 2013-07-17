using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Views.Cells;

namespace SmartWalk.iOS.Views
{
    public partial class HomeView : TableViewBase
    {
        public new HomeViewModel ViewModel
        {
            get { return (HomeViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override UITableView TableView { get { return OrgTableView; } }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
            NavigationController.NavigationBar.TintColor = UIColor.Gray;
        }

        protected override void UpdateViewTitle()
        {
            NavigationItem.Title = ViewModel.Location;
        }

        protected override MvxTableViewSource CreateTableViewSource()
        {
            var tableSource = new OrgTableSource(OrgTableView, ViewModel);

            this.CreateBinding(tableSource).To((HomeViewModel vm) => vm.OrgInfos).Apply();

            return tableSource;
        }
    }

    public class OrgTableSource : MvxTableViewSource
    {
        private readonly HomeViewModel _viewModel;

        public OrgTableSource(UITableView tableView, HomeViewModel homeViewModel)
            : base(tableView)
        {
            _viewModel = homeViewModel;

            UseAnimations = true;

            tableView.RegisterNibForCellReuse(OrgCell.Nib, OrgCell.Key);
        }

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 80.0f;
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
            return tableView.DequeueReusableCell(OrgCell.Key, indexPath);
        }
    }
}
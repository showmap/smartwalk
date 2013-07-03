using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Views.Cells;
using SmartWalk.Core.Model;

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
        }
    }

    public class OrgTableSource : MvxTableViewSource
    {
        private HomeViewModel _homeViewModel;

        public OrgTableSource(UITableView tableView, HomeViewModel homeViewModel)
            : base(tableView)
        {
            _homeViewModel = homeViewModel;

            UseAnimations = true;
            AddAnimation = UITableViewRowAnimation.Top;
            RemoveAnimation = UITableViewRowAnimation.Middle;

            tableView.RegisterNibForCellReuse(OrgInfoCell.Nib, OrgInfoCell.Key);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var org = (OrgInfo)GetItemAt(indexPath);
            if (_homeViewModel.ShowOrgViewCommand.CanExecute(org))
            {
                _homeViewModel.ShowOrgViewCommand.Execute(org);
            }
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            return tableView.DequeueReusableCell(OrgInfoCell.Key, indexPath);
        }
    }
}
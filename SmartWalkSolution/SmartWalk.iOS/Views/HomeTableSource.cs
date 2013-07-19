using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Views.Cells;

namespace SmartWalk.iOS.Views
{
    public class HomeTableSource : MvxTableViewSource
    {
        private readonly HomeViewModel _viewModel;

        public HomeTableSource(UITableView tableView, HomeViewModel homeViewModel)
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
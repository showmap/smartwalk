using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.UIKit;
using SmartWalk.Core.ViewModels;

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
            var tableSource = new HomeTableSource(OrgTableView, ViewModel);

            this.CreateBinding(tableSource).To((HomeViewModel vm) => vm.OrgInfos).Apply();

            return tableSource;
        }
    }
}
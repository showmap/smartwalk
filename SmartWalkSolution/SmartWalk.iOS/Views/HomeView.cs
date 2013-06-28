using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.ViewModels;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.Binding.Bindings;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Binders;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using SmartWalk.iOS.Views.Cells;

namespace SmartWalk.iOS.Views
{
    public partial class HomeView : MvxViewController
    {
        public HomeView() : base ("HomeView", null)
        {
        }

        public new IHomeViewModel ViewModel
        {
            get { return (IHomeViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var tableSource = new OrgTableSource(OrgTableView);

            this.CreateBinding(tableSource).To((IHomeViewModel vm) => vm.Organizations).Apply();

            OrgTableView.Source = tableSource;
            OrgTableView.ReloadData();
        }
    }

    public class OrgTableSource : MvxTableViewSource
    {
        public OrgTableSource(UITableView tableView)
            : base(tableView)
        {
            UseAnimations = true;
            AddAnimation = UITableViewRowAnimation.Top;
            RemoveAnimation = UITableViewRowAnimation.Middle;

            tableView.RegisterNibForCellReuse(OrgCell.Nib, OrgCell.Key);
        }

        protected override UITableViewCell GetOrCreateCellFor (UITableView tableView, NSIndexPath indexPath, object item)
        {
            return (UITableViewCell)tableView.DequeueReusableCell(OrgCell.Key, indexPath);
        }
    }
}
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Views.Common;
using SmartWalk.iOS.Controls;
using System.Drawing;
using SmartWalk.iOS.Utils;

namespace SmartWalk.iOS.Views.HomeView
{
    public partial class HomeView : ListViewBase
    {
        public new HomeViewModel ViewModel
        {
            get { return (HomeViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
            NavigationController.NavigationBar.TintColor = UIColor.Gray;

            SetCellWidth();
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            SetCellWidth();
        }

        protected override ListViewDecorator GetListView()
        { 
            return new ListViewDecorator(OrgCollectionView);  
        }

        protected override void UpdateViewTitle()
        {
            NavigationItem.Title = ViewModel.Location;
        }

        protected override void InitializeListView()
        {
            base.InitializeListView();

            OrgCollectionView.Delegate = new HomeCollectionDelegate(
                ViewModel, 
                (HomeCollectionSource)OrgCollectionView.Source);
        }

        protected override object CreateListViewSource()
        {
            var collectionSource = new HomeCollectionSource(OrgCollectionView);

            this.CreateBinding(collectionSource).To((HomeViewModel vm) => vm.OrgInfos).Apply();

            return collectionSource;
        }

        private void SetCellWidth()
        {
            var flowLayout = (UICollectionViewFlowLayout)OrgCollectionView.CollectionViewLayout;
            var itemsInRow = ScreenUtil.IsVerticalOrientation ? 1 : 2;

            var cellWith = (ScreenUtil.CurrentScreenWidth - 
                            flowLayout.SectionInset.Left -
                            flowLayout.SectionInset.Right - 
                            flowLayout.MinimumInteritemSpacing * (itemsInRow - 1)) / itemsInRow;

            flowLayout.ItemSize = new SizeF(cellWith, 80);
        }
    }
}
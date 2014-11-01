using System.Drawing;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common.Base;

namespace SmartWalk.Client.iOS.Views.HomeView
{
    public partial class HomeView : ListViewBase
    {
        public HomeView()
        {
            IsBackButtonVisible = false;
            IsMoreButtonVisible = false;
        }

        public new HomeViewModel ViewModel
        {
            get { return (HomeViewModel)base.ViewModel; }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            UpdateLayoutSizes(UIApplication.SharedApplication.StatusBarOrientation, View.Frame.Size);
        }

        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);

            UpdateLayoutSizes(
                toInterfaceOrientation, 
                new SizeF(View.Frame.Size.Height, View.Frame.Size.Width));
        }

        protected override ListViewDecorator GetListView()
        { 
            return ListViewDecorator.Create(OrgCollectionView);
        }

        protected override UIView GetProgressViewContainer()
        { 
            return ProgressViewContainer;  
        }

        protected override void InitializeListView()
        {
            base.InitializeListView();

            OrgCollectionView.Delegate = new HomeCollectionDelegate(
                OrgCollectionView,
                ViewModel, 
                (HomeCollectionSource)OrgCollectionView.Source);
        }

        protected override IListViewSource CreateListViewSource()
        {
            var collectionSource = 
                new HomeCollectionSource(OrgCollectionView, ViewModel);

            this.CreateBinding(collectionSource)
                .To<HomeViewModel>(vm => vm.EventInfos)
                .Apply();

            return collectionSource;
        }

        protected override void ScrollViewToTop()
        {
            var collectionDelegate = OrgCollectionView.WeakDelegate as HomeCollectionDelegate;
            if (collectionDelegate != null)
            {
                collectionDelegate.ScrollOutHeader();
            }
            else
            {
                base.ScrollViewToTop();
            }
        }

        private void UpdateLayoutSizes(UIInterfaceOrientation orientation, SizeF frameSize)
        {
            var flowLayout = (UICollectionViewFlowLayout)OrgCollectionView.CollectionViewLayout;
            var itemsInRow = ScreenUtil.GetIsVerticalOrientation(orientation) ? 1 : 2;

            var cellWidth = 
                (frameSize.Width -
                    flowLayout.SectionInset.Left -
                    flowLayout.SectionInset.Right -
                    flowLayout.MinimumInteritemSpacing * (itemsInRow - 1)) / itemsInRow;

            var cellHeight = ScreenUtil.GetProportionalHeight(
                new SizeF(OrgCell.DefaultWidth, OrgCell.DefaultHeight), 
                frameSize);

            flowLayout.ItemSize = new SizeF(cellWidth, cellHeight);
            flowLayout.HeaderReferenceSize = new SizeF(
                frameSize.Width, 
                HomeHeaderView.DefaultHeight);
            flowLayout.InvalidateLayout();
        }
    }
}
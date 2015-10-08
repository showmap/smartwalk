using System;
using Cirrious.MvvmCross.Binding.BindingContext;
using CoreGraphics;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common;
using SmartWalk.Client.iOS.Views.Common.Base;
using UIKit;

namespace SmartWalk.Client.iOS.Views.HomeView
{
    public partial class HomeView : ListViewBase
    {
        private float? _cellProportionalHeight;
        private CGSize _lastLayoutSize;

        public HomeView()
        {
            IsBackButtonVisible = false;
            IsMoreButtonVisible = false;
        }

        public new HomeViewModel ViewModel
        {
            get { return (HomeViewModel)base.ViewModel; }
        }

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }

        private float CellProportionalHeight
        {
            get
            {
                if (_cellProportionalHeight == null)
                {
                    var frameWidth = (float)Math.Min(View.Frame.Width, View.Frame.Height);

                    _cellProportionalHeight = ScreenUtil.GetProportionalHeight(
                        new CGSize(OrgCell.DefaultWidth, OrgCell.DefaultHeight), 
                        frameWidth);
                }

                return _cellProportionalHeight.Value;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = ThemeColors.ContentDarkBackground;
            ProgressView.IndicatorStyle = UIActivityIndicatorViewStyle.White;
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();

            if (_lastLayoutSize != View.Bounds.Size)
            {
                UpdateLayoutSizes(View.Bounds.Size.ToOrientation(), View.Bounds.Width);
                _lastLayoutSize = View.Bounds.Size;
            }
        }

        protected override ListViewDecorator GetListView()
        { 
            return ListViewDecorator.Create(OrgCollectionView);
        }

        protected override ProgressView GetProgressView()
        { 
            return ProgressView;  
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

        protected override void ScrollViewToTop(bool animated)
        {
            var collectionDelegate = OrgCollectionView.WeakDelegate as HomeCollectionDelegate;
            if (collectionDelegate != null)
            {
                collectionDelegate.ScrollOutHeader(animated);
            }
            else
            {
                base.ScrollViewToTop(animated);
            }
        }

        private void UpdateLayoutSizes(Orientation orientation, nfloat frameWidth)
        {
            var flowLayout = (UICollectionViewFlowLayout)OrgCollectionView.CollectionViewLayout;
            var itemsInRow = orientation == Orientation.Portrait ? 1 : 2;

            var cellWidth = 
                (frameWidth -
                    flowLayout.SectionInset.Left -
                    flowLayout.SectionInset.Right -
                    flowLayout.MinimumInteritemSpacing * (itemsInRow - 1)) / itemsInRow;

            flowLayout.ItemSize = new CGSize(cellWidth, CellProportionalHeight);
            flowLayout.HeaderReferenceSize = new CGSize(
                frameWidth, 
                HomeHeaderView.DefaultHeight);
            flowLayout.InvalidateLayout();
        }
    }
}
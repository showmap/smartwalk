using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
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

        protected override ListViewDecorator GetListView()
        { 
            return ListViewDecorator.Create(OrgCollectionView);
        }

        protected override UIView GetProgressViewContainer()
        { 
            return ProgressViewContainer;  
        }

        protected override NSLayoutConstraint GetProgressViewTopConstraint()
        {
            return ProgressViewTopConstraint;
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
    }
}
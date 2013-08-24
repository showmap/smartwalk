using System.ComponentModel;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.UIKit;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Controls;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.HomeView
{
    public partial class HomeView : ListViewBase
    {
        private MvxImageViewLoader _imageHelper;

        public HomeView()
        {
            _imageHelper = new MvxImageViewLoader(() => BackgroundImageView);

            var set = this.CreateBindingSet<HomeView, HomeViewModel>();
            set.Bind(_imageHelper).To(vm => vm.Location.Logo);
            set.Apply();
        }

        public new HomeViewModel ViewModel
        {
            get { return (HomeViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
            NavigationController.NavigationBar.TintColor = UIColor.LightGray;

            BackgroundImageView.ClipsToBounds = true;
            OrgCollectionView.BackgroundColor = null;
            OrgCollectionView.CellHeight = 80;
        }

        protected override ListViewDecorator GetListView()
        { 
            return new ListViewDecorator(OrgCollectionView);  
        }

        protected override UIView GetProgressViewContainer()
        { 
            return ProgressViewContainer;  
        }

        protected override void UpdateViewTitle()
        {
            NavigationItem.Title = ViewModel.Location != null 
                ? ViewModel.Location.Name 
                : string.Empty;
        }

        protected override void InitializeListView()
        {
            base.InitializeListView();

            OrgCollectionView.Delegate = new HomeCollectionDelegate(
                ViewModel, 
                (HomeCollectionSource)OrgCollectionView.Source);
        }

        protected override IListViewSource CreateListViewSource()
        {
            var collectionSource = new HomeCollectionSource(OrgCollectionView);

            this.CreateBinding(collectionSource).To((HomeViewModel vm) => vm.OrgInfos).Apply();

            return collectionSource;
        }

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            if (propertyName == ViewModel.GetPropertyName(p => p.Location))
            {
                UpdateViewTitle();
            }
        }
    }
}
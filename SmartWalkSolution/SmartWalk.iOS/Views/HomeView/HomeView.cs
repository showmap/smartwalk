using System.ComponentModel;
using System.Drawing;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.UIKit;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Controls;
using SmartWalk.iOS.Utils;
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
            NavigationController.NavigationBar.TintColor = UIColor.LightGray;  //UIColor.FromRGB(230, 230, 230);

            BackgroundImageView.ClipsToBounds = true;
            OrgCollectionView.BackgroundColor = null;

            /*var attr = new UITextAttributes();
            attr.TextColor = UIColor.Gray;
            attr.TextShadowColor = UIColor.White;
            NavigationController.NavigationBar.SetTitleTextAttributes(attr);*/

            SetCellWidth();
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            SetCellWidth();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            SetCellWidth();
        }

        protected override ListViewDecorator GetListView()
        { 
            return new ListViewDecorator(OrgCollectionView);  
        }

        protected override void UpdateViewTitle()
        {
            NavigationItem.Title = ViewModel.Location != null ? ViewModel.Location.Name : string.Empty;
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

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModel.GetPropertyName(p => p.Location))
            {
                UpdateViewTitle();
            }
        }

        private void SetCellWidth()
        {
            var flowLayout = (UICollectionViewFlowLayout)OrgCollectionView.CollectionViewLayout;
            var itemsInRow = ScreenUtil.IsVerticalOrientation ? 1 : 2;

            var cellWith = (ScreenUtil.CurrentScreenSize.Width - 
                            flowLayout.SectionInset.Left -
                            flowLayout.SectionInset.Right - 
                            flowLayout.MinimumInteritemSpacing * (itemsInRow - 1)) / itemsInRow;

            flowLayout.ItemSize = new SizeF(cellWith, 80);
        }
    }
}
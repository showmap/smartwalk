using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Controls;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.HomeView
{
    public partial class HomeView : ListViewBase
    {
        public new HomeViewModel ViewModel
        {
            get { return (HomeViewModel)base.ViewModel; }
        }

        protected override ListViewDecorator GetListView()
        { 
            return new ListViewDecorator(OrgCollectionView);
        }

        protected override UIView GetProgressViewContainer()
        { 
            return ProgressViewContainer;  
        }

        protected override string GetViewTitle()
        {
            return ViewModel.Location != null ? ViewModel.Location.Name : null;
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
                GetViewTitle();
            }
        }
    }
}
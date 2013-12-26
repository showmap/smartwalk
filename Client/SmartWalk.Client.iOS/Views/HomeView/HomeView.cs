using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Views.Common;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Views.HomeView
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
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Views.Common.Base;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
    public partial class OrgEventInfoView : ListViewBase
    {
        public new OrgEventInfoViewModel ViewModel
        {
            get { return (OrgEventInfoViewModel)base.ViewModel; }
        }

        protected override ListViewDecorator GetListView()
        { 
            return new ListViewDecorator(OrgEventInfoTableView);  
        }

        protected override UIView GetProgressViewContainer()
        { 
            return ProgressViewContainer;
        }

        protected override NSLayoutConstraint GetProgressViewTopConstraint()
        {
            return ProgressViewTopConstraint;
        }

        protected override IListViewSource CreateListViewSource()
        {
            var tableSource = new OrgEventInfoTableSource(OrgEventInfoTableView, ViewModel);

            this.CreateBinding(tableSource)
                .To<OrgEventInfoViewModel>(vm => vm.OrgEvent)
                .WithConversion(new OrgEventInfoTableSourceConverter(), ViewModel)
                .Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            if (propertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                OrgEventInfoTableView.BeginUpdates();
                OrgEventInfoTableView.EndUpdates();
            }
        }
    }
}
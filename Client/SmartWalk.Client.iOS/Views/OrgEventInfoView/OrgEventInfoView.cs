using Cirrious.MvvmCross.Binding.BindingContext;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common;
using SmartWalk.Client.iOS.Views.Common.Base;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.iOS.Views.OrgEventInfoView
{
    public partial class OrgEventInfoView : EntityViewBase
    {
        public new OrgEventInfoViewModel ViewModel
        {
            get { return (OrgEventInfoViewModel)base.ViewModel; }
        }

        protected override ListViewDecorator GetListView()
        { 
            return ListViewDecorator.Create(OrgEventInfoTableView);  
        }

        protected override ProgressView GetProgressView()
        { 
            return ProgressView;
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
            base.OnViewModelPropertyChanged(propertyName);

            if (propertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                OrgEventInfoTableView.UpdateLayout();
            }
        }
    }
}
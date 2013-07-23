using System.ComponentModel;
using Cirrious.MvvmCross.Binding.BindingContext;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Views.Common;
using SmartWalk.iOS.Controls;

namespace SmartWalk.iOS.Views.OrgView
{
    public partial class OrgView : ListViewBase
    {
        public new OrgViewModel ViewModel
        {
            get { return (OrgViewModel)base.ViewModel; }
        }

        protected override void UpdateViewTitle()
        {
            if (ViewModel.Org != null && ViewModel.Org.Info != null)
            {
                NavigationItem.Title = ViewModel.Org.Info.Name;
            }
        }

        protected override ListViewDecorator GetListView()
        { 
            return new ListViewDecorator(OrgEventsTableView);  
        }

        protected override object CreateListViewSource()
        {
            var tableSource = new OrgTableSource(OrgEventsTableView, ViewModel);

            this.CreateBinding(tableSource).To((OrgViewModel vm) => vm)
                .WithConversion(new OrgTableSourceConverter(), null).Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                OrgEventsTableView.BeginUpdates();
                OrgEventsTableView.EndUpdates();
            }
        }
    }
}
using System.ComponentModel;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.UIKit;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.OrgView
{
    public partial class OrgView : TableViewBase
    {
        public new OrgViewModel ViewModel
        {
            get { return (OrgViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override UITableView TableView { get { return OrgEventsTableView; } }

        protected override void UpdateViewTitle()
        {
            if (ViewModel.Org != null && ViewModel.Org.Info != null)
            {
                NavigationItem.Title = ViewModel.Org.Info.Name;
            }
        }

        protected override MvxTableViewSource CreateTableViewSource()
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
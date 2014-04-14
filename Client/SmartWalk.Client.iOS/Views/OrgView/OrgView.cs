using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Views.Common;

namespace SmartWalk.Client.iOS.Views.OrgView
{
    public partial class OrgView : ListViewBase
    {
        public new OrgViewModel ViewModel
        {
            get { return (OrgViewModel)base.ViewModel; }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // HACK: to fix the bug with floating tableview
            if (OrgEventsTableView.VisibleCells.Length > 0)
            {
                OrgEventsTableView.BeginUpdates();
                OrgEventsTableView.EndUpdates();
            }
        }

        protected override string GetViewTitle()
        {
            if (ViewModel.Org != null && ViewModel.Org.Info != null)
            {
                return ViewModel.Org.Info.Name;
            }

            return null;
        }

        protected override ListViewDecorator GetListView()
        { 
            return new ListViewDecorator(OrgEventsTableView);  
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
            var tableSource = new OrgTableSource(OrgEventsTableView, ViewModel);

            this.CreateBinding(tableSource)
                .To<OrgViewModel>(vm => vm.Org)
                .WithConversion(new OrgTableSourceConverter(), ViewModel)
                .Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            if (propertyName == ViewModel.GetPropertyName(vm => vm.Org))
            {
                GetViewTitle();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                OrgEventsTableView.BeginUpdates();
                OrgEventsTableView.EndUpdates();
            }
        }
    }
}
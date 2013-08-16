using System.ComponentModel;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.ViewModels;
using MonoTouch.UIKit;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Controls;
using SmartWalk.iOS.Views.Common;

namespace SmartWalk.iOS.Views.OrgView
{
    public partial class OrgView : ListViewBase
    {
        public new OrgViewModel ViewModel
        {
            get { return (OrgViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeToolBar();
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseDesignerOutlets();
            base.Dispose(disposing);
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

            tableSource.ShowImageFullscreenCommand = new MvxCommand<string>(ShowImageFullscreenView);

            this.CreateBinding(tableSource).To((OrgViewModel vm) => vm.Org)
                .WithConversion(new OrgTableSourceConverter(), ViewModel).Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.Org))
            {
                UpdateViewTitle();
            }
            else if (e.PropertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                OrgEventsTableView.BeginUpdates();
                OrgEventsTableView.EndUpdates();
            }
        }

        private void InitializeToolBar()
        {
            var buttonUp = new UIBarButtonItem("↑", UIBarButtonItemStyle.Plain, (s, e) => 
                {
                    if (ViewModel.ShowPreviousEntityCommand.CanExecute(null))
                    {
                        ViewModel.ShowPreviousEntityCommand.Execute(null);
                    }
                });
            var buttonDown = new UIBarButtonItem("↓", UIBarButtonItemStyle.Plain, (s, e) => 
                {
                    if (ViewModel.ShowNextEntityCommand.CanExecute(null))
                    {
                        ViewModel.ShowNextEntityCommand.Execute(null);
                    }
                });

            NavigationItem.SetRightBarButtonItems(new [] {buttonDown, buttonUp}, true);
        }
    }
}
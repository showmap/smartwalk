using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Views.Common.Base;

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

            // HACK: to fix the bug with floating tableview, probably iOS 6 only
            if (OrgEventsTableView.VisibleCells.Length > 0)
            {
                OrgEventsTableView.BeginUpdates();
                OrgEventsTableView.EndUpdates();
            }
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
            base.OnViewModelPropertyChanged(propertyName);

            if (propertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                OrgEventsTableView.BeginUpdates();
                OrgEventsTableView.EndUpdates();
            }
        }

        protected override void OnInitializingActionSheet(UIActionSheet actionSheet)
        {
            if (ViewModel.ShowHideContactsCommand.CanExecute(ViewModel.Entity))
            {
                actionSheet.AddButton(Localization.ShowContactInfo);
            }

            if (ViewModel.CopyLinkCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.CopyLink);
            }

            if (ViewModel.ShareCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.ShareButton);
            }
        }

        protected override void OnActionSheetClick(string buttonTitle)
        {
            switch (buttonTitle)
            {
                case Localization.ShowContactInfo:
                    if (ViewModel.ShowHideContactsCommand.CanExecute(ViewModel.Entity))
                    {
                        ViewModel.ShowHideContactsCommand.Execute(ViewModel.Entity);
                    }
                    break;

                case Localization.CopyLink:
                    if (ViewModel.CopyLinkCommand.CanExecute(null))
                    {
                        ViewModel.CopyLinkCommand.Execute(null);
                    }
                    break;

                case Localization.ShareButton:
                    if (ViewModel.ShareCommand.CanExecute(null))
                    {
                        ViewModel.ShareCommand.Execute(null);
                    }
                    break;
            }
        }
    }
}
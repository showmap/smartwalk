using System;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common;

namespace SmartWalk.Client.iOS.Views.OrgView
{
    public partial class OrgView : ListViewBase
    {
        private ButtonBarButton _moreButton;

        public new OrgViewModel ViewModel
        {
            get { return (OrgViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeToolBar();
        }

        public override void WillMoveToParentViewController(UIViewController parent)
        {
            base.WillMoveToParentViewController(parent);

            if (parent == null)
            {
                DisposeToolBar();
            }
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

        private void InitializeToolBar()
        {
            var spacer = ButtonBarUtil.CreateSpacer();

            _moreButton = ButtonBarUtil.Create(ThemeIcons.NavBarMore, ThemeIcons.NavBarMoreLandscape);
            _moreButton.TouchUpInside += OnMoreButtonClicked;

            var moreBarButton = new UIBarButtonItem(_moreButton);
            NavigationItem.SetRightBarButtonItems(new [] {spacer, moreBarButton}, true);
        }

        private void OnMoreButtonClicked(object sender, EventArgs e)
        {
            var actionSheet = new UIActionSheet();
            actionSheet.Style = UIActionSheetStyle.BlackTranslucent;
            actionSheet.Clicked += OnActionClicked;

            if (ViewModel.ShowHideContactsCommand.CanExecute(ViewModel.Entity))
            {
                actionSheet.AddButton(Localization.ShowContactInfo);
            }

            if (ViewModel.CopyLinkCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.CopyLink);
                actionSheet.AddButton(Localization.ShareButton);
            }

            actionSheet.AddButton(Localization.CancelButton);

            actionSheet.CancelButtonIndex = actionSheet.ButtonCount - 1;

            actionSheet.ShowInView(View);
        }

        private void OnActionClicked(object sender, UIButtonEventArgs e)
        {
            var actionSheet = ((UIActionSheet)sender);
            actionSheet.Clicked -= OnActionClicked;

            switch (actionSheet.ButtonTitle(e.ButtonIndex))
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
                    // TODO: Share Host Link
                    break;
            }
        }

        private void DisposeToolBar()
        {
            if (_moreButton != null)
            {
                _moreButton.TouchUpInside -= OnMoreButtonClicked;
            }
        }
    }
}
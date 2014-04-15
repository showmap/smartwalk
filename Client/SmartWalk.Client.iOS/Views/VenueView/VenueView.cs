using System;
using System.Linq;
using Cirrious.MvvmCross.Binding.BindingContext;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Views.Common;
using SmartWalk.Client.iOS.Views.OrgEventView;

namespace SmartWalk.Client.iOS.Views.VenueView
{
    public partial class VenueView : ListViewBase
    {
        private ButtonBarButton _moreButton;

        public new VenueViewModel ViewModel
        {
            get { return (VenueViewModel)base.ViewModel; }
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

        protected override string GetViewTitle()
        {
            if (ViewModel.Venue != null && ViewModel.Venue.Info != null)
            {
                return ViewModel.Venue.Info.Name;
            }

            return null;
        }

        protected override ListViewDecorator GetListView()
        { 
            return new ListViewDecorator(VenueShowsTableView);  
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
            var tableSource = new VenueTableSource(VenueShowsTableView, ViewModel);

            this.CreateBinding(tableSource)
                .To<VenueViewModel>(vm => vm.Venue)
                .WithConversion(new VenueTableSourceConverter(), ViewModel)
                .Apply();

            return tableSource;
        }

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            if (propertyName == ViewModel.GetPropertyName(vm => vm.Venue))
            {
                GetViewTitle();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.IsDescriptionExpanded))
            {
                VenueShowsTableView.BeginUpdates();
                VenueShowsTableView.EndUpdates();
            }
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.ExpandedShow))
            {
                foreach (var cell in VenueShowsTableView.VisibleCells.OfType<VenueShowCell>())
                {
                    cell.IsExpanded = Equals(cell.DataContext, ViewModel.ExpandedShow);
                }

                VenueShowsTableView.BeginUpdates();
                VenueShowsTableView.EndUpdates();
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

            if (ViewModel.ShowDirectionsCommand.CanExecute(ViewModel.Entity))
            {
                actionSheet.AddButton(Localization.NavigateInMaps);
            }

            if (ViewModel.ShowHideContactsCommand.CanExecute(ViewModel.Entity))
            {
                actionSheet.AddButton(Localization.ShowContactInfo);
            }

            if (ViewModel.CopyAddressCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.CopyAddress);
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
                case Localization.NavigateInMaps:
                    if (ViewModel.ShowDirectionsCommand.CanExecute(ViewModel.Entity))
                    {
                        ViewModel.ShowDirectionsCommand.Execute(ViewModel.Entity);
                    }
                    break;

                case Localization.ShowContactInfo:
                    if (ViewModel.ShowHideContactsCommand.CanExecute(ViewModel.Entity))
                    {
                        ViewModel.ShowHideContactsCommand.Execute(ViewModel.Entity);
                    }
                    break;

                case Localization.CopyAddress:
                    if (ViewModel.CopyAddressCommand.CanExecute(null))
                    {
                        ViewModel.CopyAddressCommand.Execute(null);
                    }
                    break;

                case Localization.CopyLink:
                    if (ViewModel.CopyLinkCommand.CanExecute(null))
                    {
                        ViewModel.CopyLinkCommand.Execute(null);
                    }
                    break;

                case Localization.ShareButton:
                    // TODO: Share Venue Link
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
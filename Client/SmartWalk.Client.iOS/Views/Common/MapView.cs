using System;
using System.ComponentModel;
using System.Linq;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.Map;
using SmartWalk.Client.iOS.Views.Common.Base;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class MapView : ActiveAwareViewController
    {
        private ButtonBarButton _moreButton;

        public new MapViewModel ViewModel
        {
            get { return (MapViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeToolBar();

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                MapViewControl.TintColor = Theme.MapTint;
            }

            MapViewControl.Delegate = new MapDelegate { CanShowDetails = false };

            UpdateViewTitle();
            SelectAnnotation();

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModel.GetPropertyName(p => p.Annotation))
            {
                UpdateViewTitle();
                SelectAnnotation();
            }
        }

        private void SelectAnnotation()
        {
            MapViewControl.RemoveAnnotations(MapViewControl.Annotations);

            if (ViewModel.Annotation != null &&
                ViewModel.Annotation.Addresses != null &&
                ViewModel.Annotation.Addresses.Length > 0)
            {
                var annotations = ViewModel.Annotation.Addresses
                    .Select(address => new MapViewAnnotation(
                        ViewModel.Annotation.Title,
                        address)).ToArray();
                var coordinates = MapUtil.GetAnnotationsCoordinates(annotations);

                MapViewControl.SetRegion(
                    MapUtil.CoordinateRegionForCoordinates(
                        coordinates,
                        new MKMapSize(5000, 5000)), 
                    false);
                MapViewControl.AddAnnotations(annotations);
                MapViewControl.SelectAnnotation(annotations[0], false);
            }
        }

        private void UpdateViewTitle()
        {
            NavigationItem.Title = ViewModel.Annotation != null
                ? ViewModel.Annotation.Title
                : null;
        }

        private void InitializeToolBar()
        {
            ButtonBarUtil.OverrideNavigatorBackButton(
                NavigationItem,
                () => NavigationController.PopViewControllerAnimated(true));

            var spacer = ButtonBarUtil.CreateSpacer();

            _moreButton = ButtonBarUtil.Create(ThemeIcons.NavBarMore, ThemeIcons.NavBarMoreLandscape);
            _moreButton.TouchUpInside += OnMoreButtonClicked;

            var moreBarButton = new UIBarButtonItem(_moreButton);
            NavigationItem.SetRightBarButtonItems(new [] {spacer, moreBarButton}, true);
        }

        private void DisposeToolBar()
        {
            if (_moreButton != null)
            {
                _moreButton.TouchUpInside -= OnMoreButtonClicked;
            }
        }

        private void OnMoreButtonClicked(object sender, EventArgs e)
        {
            var actionSheet = ActionSheetUtil.CreateActionSheet(OnActionClicked);

            if (ViewModel.ShowDirectionsCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.NavigateInMaps);
            }

            if (ViewModel.CopyAddressCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.CopyAddress);
            }

            if (true)
            {
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
                    if (ViewModel.ShowDirectionsCommand.CanExecute(null))
                    {
                        ViewModel.ShowDirectionsCommand.Execute(null);
                    }
                    break;

                case Localization.CopyAddress:
                    if (ViewModel.CopyAddressCommand.CanExecute(null))
                    {
                        ViewModel.CopyAddressCommand.Execute(null);
                    }
                    break;

                case Localization.ShareButton:
                    // TODO: Share Address Text
                    break;
            }
        }
    }
}
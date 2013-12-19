using System;
using System.ComponentModel;
using System.Linq;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.Map;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class MapView : ActiveAwareViewController
    {
        public new MapViewModel ViewModel
        {
            get { return (MapViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeTopToolBar();
            InitializeStyle();

            MapViewControl.Delegate = new MapDelegate { CanShowDetails = false };

            UpdateViewTitle();
            SelectAnnotation();
            UpdateBottomToolBarState();

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.LeftBarButtonItems);
            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.RightBarButtonItems);
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.LeftBarButtonItems);
            ButtonBarUtil.UpdateButtonsFrameOnRotation(NavigationItem.RightBarButtonItems);
            UpdateBottomToolBarState();
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
                UpdateBottomToolBarState();
            }
        }

        private void SelectAnnotation()
        {
            MapViewControl.RemoveAnnotations(MapViewControl.Annotations);

            if (ViewModel.Annotation != null &&
                ViewModel.Annotation.AddressInfos != null &&
                ViewModel.Annotation.AddressInfos.Length > 0)
            {
                var annotations = ViewModel.Annotation.AddressInfos
                    .Select(address => new MapViewAnnotation(
                        ViewModel.Annotation.Number,
                        ViewModel.Annotation.Title,
                        address)).ToArray();
                var coordinates = MapUtil.GetAnnotationsCoordinates(annotations);

                MapViewControl.SetRegion(
                    MapUtil.CoordinateRegionForCoordinates(
                        coordinates,
                        new MKMapSize(5000, 5000)), 
                    false);
                MapViewControl.AddAnnotations(annotations);
                MapViewControl.SelectAnnotation(annotations.First(), false);
            }
        }

        private void UpdateViewTitle()
        {
            NavigationItem.Title = ViewModel.Annotation != null
                ? ViewModel.Annotation.Title
                : null;
        }

        private void UpdateBottomToolBarState()
        {
            var currentAddress = GetCurrentAddress();

            if (currentAddress != null)
            {
                BottomToolBarHeightConstraint.Constant = 
                    ScreenUtil.IsVerticalOrientation ? 44 : 33;

                AddressLabel.Text = currentAddress;
            }
            else
            {
                BottomToolBarHeightConstraint.Constant = 0;
            }

            CopyButton.Font = ScreenUtil.IsVerticalOrientation 
                ? Theme.ButtonTextFont
                : Theme.ButtonTextLandscapeFont;
        }

        private void InitializeTopToolBar()
        {
            ButtonBarUtil.OverrideNavigatorBackButton(
                NavigationItem,
                () => NavigationController.PopViewControllerAnimated(true));

            var navigateButton = ButtonBarUtil.Create(ThemeIcons.NavBarNavigate, ThemeIcons.NavBarNavigateLandscape);
            navigateButton.TouchUpInside += (sender, e) => 
                { 
                    if (ViewModel.Annotation != null &&
                        ViewModel.Annotation.AddressInfos != null)
                    {
                        var addressInfo = ViewModel.Annotation.AddressInfos.FirstOrDefault();
                        
                        if (ViewModel.ShowDirectionsCommand.CanExecute(addressInfo))
                        {
                            ViewModel.ShowDirectionsCommand.Execute(addressInfo);
                        }
                    }
                };

            var navigationBarButton = new UIBarButtonItem(navigateButton);
            NavigationItem.SetRightBarButtonItems(
                new [] { ButtonBarUtil.CreateSpacer(), navigationBarButton }, true);  
        }

        private void InitializeStyle()
        {
            AddressLabel.Font = Theme.MapViewAddressFont;

            CopyButton.Layer.BorderWidth = 1;
            CopyButton.Layer.CornerRadius = 4;
            CopyButton.Layer.BorderColor = UIColor.White.CGColor;

            CopyButton.TouchDown += (sender, e) => CopyButton.BackgroundColor = UIColor.Black;
            CopyButton.TouchUpInside += (sender, e) => CopyButton.BackgroundColor = UIColor.Clear;
            CopyButton.TouchUpOutside += (sender, e) => CopyButton.BackgroundColor = UIColor.Clear;

            BottomToolBarView.BackgroundColor = Theme.NavBarBackground;
        }

        partial void OnCopyButtonClick(UIButton sender, UIEvent @event)
        {
            var currentAddress = GetCurrentAddress();
            if (currentAddress != null)
            {
                UIPasteboard.General.String = currentAddress;
            }
        }

        private string GetCurrentAddress()
        {
            if (ViewModel.Annotation != null &&
                ViewModel.Annotation.AddressInfos != null)
            {
                var info = ViewModel.Annotation.AddressInfos
                    .FirstOrDefault(ai => !string.IsNullOrWhiteSpace(ai.Address));
                return info != null && !string.IsNullOrWhiteSpace(info.Address) ? info.Address : null; 
            }

            return null;
        }
    }
}
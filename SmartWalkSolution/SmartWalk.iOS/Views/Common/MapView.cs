using System.ComponentModel;
using System.Linq;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Utils;
using SmartWalk.iOS.Utils.Map;
using SmartWalk.iOS.Resources;

namespace SmartWalk.iOS.Views.Common
{
    public partial class MapView : ActiveAwareController
    {
        public new MapViewModel ViewModel
        {
            get { return (MapViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ButtonBarUtil.OverrideNavigatorBackButton(NavigationItem, NavigationController);
            InitializeToolBar();

            MapViewControl.Delegate = new MapDelegate { CanShowDetails = false };

            UpdateViewTitle();
            SelectAnnotation();

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
                ? ViewModel.Annotation.Title.ToUpper()
                : null;
        }

        private void InitializeToolBar()
        {
            var navigateButton = ButtonBarUtil.Create(ThemeIcons.Navigate, null);
            navigateButton.TouchUpInside += (sender, e) => 
                { 
                    if (ViewModel.Annotation != null &&
                        ViewModel.Annotation.AddressInfos != null)
                    {
                        var addressInfo = ViewModel.Annotation.AddressInfos.FirstOrDefault();
                        MapUtil.OpenAddressInMaps(addressInfo);
                    }
                };

            var barItem = new UIBarButtonItem(navigateButton);
            NavigationItem.SetRightBarButtonItems(new [] { barItem }, true);  
        }
    }
}
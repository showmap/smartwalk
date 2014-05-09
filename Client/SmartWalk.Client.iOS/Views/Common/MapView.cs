using System.Linq;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils.Map;
using SmartWalk.Client.iOS.Views.Common.Base;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class MapView : CustomNavBarViewBase
    {
        public new MapViewModel ViewModel
        {
            get { return (MapViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeStyle();

            MapViewControl.Delegate = new MapDelegate { CanShowDetails = false };

            UpdateViewTitle();
            SelectAnnotation();
        }

        protected override void OnInitializingActionSheet(UIActionSheet actionSheet)
        {
            if (ViewModel.ShowDirectionsCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.NavigateInMaps);
            }

            if (ViewModel.CopyAddressCommand.CanExecute(null))
            {
                actionSheet.AddButton(Localization.CopyAddress);
            }

            // TODO: Use command for Share
            if (true)
            {
                actionSheet.AddButton(Localization.ShareButton);
            }
        }

        protected override void OnActionSheetClick(string buttonTitle)
        {
            switch (buttonTitle)
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

        protected override void OnViewModelPropertyChanged(string propertyName)
        {
            base.OnViewModelPropertyChanged(propertyName);

            if (propertyName == ViewModel.GetPropertyName(p => p.Annotation))
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

        private void InitializeStyle()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                MapViewControl.TintColor = Theme.MapTint;
            }
        }
    }
}
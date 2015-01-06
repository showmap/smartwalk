using System.Collections.Generic;
using System.Linq;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;
using SmartWalk.Client.Core.Resources;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Shared.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils.Map;
using SmartWalk.Client.iOS.Views.Common.Base;
using SmartWalk.Client.iOS.Controls;

namespace SmartWalk.Client.iOS.Views.Common
{
    public partial class MapView : NavBarViewBase
    {
        public new MapViewModel ViewModel
        {
            get { return (MapViewModel)base.ViewModel; }
        }

        protected override string ViewTitle
        {
            get
            {
                var result = ViewModel.Annotation != null
                    ? ViewModel.Annotation.Title
                    : null;
                return result;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            InitializeStyle();

            MapViewControl.Delegate = new MapDelegate { CanShowDetails = false };

            UpdateViewTitle();
            SelectAnnotation();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            SetNavBarTransparent(SemiTransparentType.Light, animated);
        }

        protected override void OnInitializingActionSheet(List<string> titles)
        {
            if (ViewModel.ShowDirectionsCommand.CanExecute(null))
            {
                titles.Add(Localization.NavigateInMaps);
            }

            if (ViewModel.SwitchMapTypeCommand.CanExecute(null))
            {
                titles.Add(ViewModel.CurrentMapType.GetMapTypeButtonLabel());
            }

            if (ViewModel.CopyAddressCommand.CanExecute(null))
            {
                titles.Add(Localization.CopyAddress);
            }

            if (ViewModel.ShareCommand.CanExecute(null))
            {
                titles.Add(Localization.ShareButton);
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
                    if (ViewModel.ShareCommand.CanExecute(null))
                    {
                        ViewModel.ShareCommand.Execute(null);
                    }
                    break;
            }

            if (buttonTitle == ViewModel.CurrentMapType.GetMapTypeButtonLabel())
            {
                if (ViewModel.SwitchMapTypeCommand.CanExecute(null))
                {
                    ViewModel.SwitchMapTypeCommand.Execute(null);
                }
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
            else if (propertyName == ViewModel.GetPropertyName(vm => vm.CurrentMapType))
            {
                MapViewControl.MapType = ViewModel.CurrentMapType.ToMKMapType();
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
                        ViewModel.Annotation.Pin,
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

        private void InitializeStyle()
        {
            MapViewControl.TintColor = Theme.HeaderText;
        }
    }
}
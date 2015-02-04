using System.Windows.Input;
using CoreGraphics;
using MapKit;
using ObjCRuntime;
using SmartWalk.Client.iOS.Controls;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils.Map;
using UIKit;

namespace SmartWalk.Client.iOS.Utils.Map
{
    public class MapDelegate : MKMapViewDelegate
    {
        private readonly string _annotationIdentifier = "BasicAnnotation";

        public MapDelegate()
        {
            CanShowCallout = true;
            CanShowDetails = true;
        }

        public bool IsMapBeingTouched { get; set; }

        public bool CanShowCallout { get; set; }
        public bool CanShowDetails { get; set; }

        public ICommand SelectAnnotationCommand { get; set; }
        public ICommand ShowDetailsCommand { get; set; }

        public override MKAnnotationView GetViewForAnnotation(
            MKMapView mapView,
            IMKAnnotation annotation)
        {
            if (IsUserLocation(annotation, mapView)) return null;

            var annotationView = mapView.DequeueReusableAnnotation(_annotationIdentifier);
            if (annotationView == null)
            {
                annotationView = 
                    new MKAnnotationView(annotation, _annotationIdentifier);
            }
            else
            {
                annotationView.Annotation = annotation;
            }

            var mapAnnotation = annotation as IMapAnnotation;
            if (mapAnnotation != null)
            {
                annotationView.CanShowCallout = CanShowCallout;
                annotationView.Image = ThemeIcons.MapPin;
                annotationView.CenterOffset = Theme.MapPinOffset;
                annotationView.CalloutOffset = new CGPoint(0, 0);
                annotationView.RemoveSubviews();

                var numberLabel = new UILabel { 
                    TextColor = ThemeColors.ContentDarkText,
                    Font = Theme.MapPinFont,
                    TextAlignment = UITextAlignment.Center,
                    BackgroundColor = UIColor.Clear,
                    Text = mapAnnotation.Pin,
                    Frame = new CGRect(Theme.MapPinTextOffset, new CGSize(25, 25))
                };
                annotationView.AddSubview(numberLabel);

                if (CanShowCallout && CanShowDetails)
                {
                    var detailButton = UIButton.FromType(UIButtonType.Custom);
                    detailButton.Frame = new CGRect(0, 0, 32, 32);
                    detailButton.ImageView.ContentMode = UIViewContentMode.Center;
                    detailButton.ClipsToBounds = false;
                    detailButton.SetImage(ThemeIcons.Forward, UIControlState.Normal);
                    detailButton.TintColor = ThemeColors.Action;

                    detailButton.TouchUpInside += (s, e) => 
                        {
                            if (ShowDetailsCommand != null &&
                                ShowDetailsCommand.CanExecute(mapAnnotation.DataContext))
                            {
                                ShowDetailsCommand.Execute(mapAnnotation.DataContext);
                            }
                        };

                    annotationView.RightCalloutAccessoryView = detailButton;
                }
            }

            return annotationView;
        }

        public override void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            var mapAnnotation = view.Annotation as IMapAnnotation;
            var customMapView = mapView as CustomMKMapView;

            if (mapAnnotation != null &&
                customMapView != null &&
                customMapView.IsBeingTouched &&
                SelectAnnotationCommand != null &&
                SelectAnnotationCommand.CanExecute(mapAnnotation.DataContext))
            {
                SelectAnnotationCommand.Execute(mapAnnotation.DataContext);
            }
        }

        public override void DidDeselectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            var customMapView = mapView as CustomMKMapView;

            if (customMapView != null &&
                customMapView.IsBeingTouched &&
                SelectAnnotationCommand != null &&
                SelectAnnotationCommand.CanExecute(null))
            {
                SelectAnnotationCommand.Execute(null);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        private static bool IsUserLocation(IMKAnnotation annotation, MKMapView mapView)
        {
            var userLocation = Runtime.GetNSObject(annotation.Handle) as MKUserLocation;
            return userLocation != null && userLocation == mapView.UserLocation;
        }
    }
}
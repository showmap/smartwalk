using System;
using System.Collections.Generic;
using System.Drawing;
using Cirrious.CrossCore.Core;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils.Map;

namespace SmartWalk.Client.iOS.Utils.Map
{
    public class MapDelegate : MKMapViewDelegate
    {
        private readonly List<MKPinAnnotationView> _viewLinksList = 
            new List<MKPinAnnotationView>(); // to prevent GC

        private readonly string _annotationIdentifier = "BasicAnnotation";

        public MapDelegate()
        {
            CanShowCallout = true;
            CanShowDetails = true;
        }

        public event EventHandler<MvxValueEventArgs<IMapAnnotation>> DetailsClick;

        public bool CanShowCallout { get; set; }
        public bool CanShowDetails { get; set; }

        public override MKAnnotationView GetViewForAnnotation(
            MKMapView mapView,
            NSObject annotation)
        {
            if (annotation is MKUserLocation)
            {
                return null;
            }

            var annotationView = (MKPinAnnotationView)mapView
                .DequeueReusableAnnotation(_annotationIdentifier);
            if (annotationView == null)
            {
                annotationView = 
                    new MKPinAnnotationView(annotation, _annotationIdentifier);
            }
            else
            {
                annotationView.Annotation = annotation;
            }

            if (!_viewLinksList.Contains(annotationView))
            {
                _viewLinksList.Add(annotationView);
            }

            var mapAnnotation = annotation as IMapAnnotation;
            if (mapAnnotation != null)
            {
                annotationView.CanShowCallout = CanShowCallout;
                annotationView.Image = ThemeIcons.MapPin;
                annotationView.CenterOffset = Theme.MapPinOffset;
                annotationView.CalloutOffset = new PointF(0, 0);
                annotationView.RemoveSubviews();

                var numberLabel = new UILabel { 
                    TextColor = Theme.MapPinText,
                    Font = Theme.MapPinFont,
                    TextAlignment = UITextAlignment.Center,
                    BackgroundColor = UIColor.Clear,
                    Text = mapAnnotation.Abbr,
                    Frame = new RectangleF(Theme.MapPinTextOffset, new SizeF(25, 25))
                };
                annotationView.AddSubview(numberLabel);

                if (CanShowCallout && CanShowDetails)
                {
                    var detailButton = UIButton.FromType(UIButtonType.Custom);
                    detailButton.Frame = new RectangleF(0, 0, 32, 32);
                    detailButton.SetImage(ThemeIcons.Details, UIControlState.Normal);
                    detailButton.TouchUpInside += (s, e) => 
                        {
                            if (DetailsClick != null)
                            {
                                DetailsClick(
                                    this, 
                                    new MvxValueEventArgs<IMapAnnotation>(mapAnnotation));
                            }
                        };

                    annotationView.RightCalloutAccessoryView = detailButton;
                }
            }

            return annotationView;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }
}
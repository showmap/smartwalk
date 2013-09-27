using System;
using System.Collections.Generic;
using System.Drawing;
//using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using SmartWalk.Core.Utils;
using SmartWalk.iOS.Resources;
using SmartWalk.iOS.Utils.Map;
using Cirrious.CrossCore.Core;

namespace SmartWalk.iOS.Utils.Map
{
    public class MapDelegate : MKMapViewDelegate
    {
        private readonly List<MKPinAnnotationView> _viewLinksList = 
            new List<MKPinAnnotationView>(); // to prevent GC

        //private MvxImageViewLoader _imageHelper;
        private string _annotationIdentifier = "BasicAnnotation";

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
                annotationView.Image = Theme.MapPinIcon;
                annotationView.CenterOffset = Theme.MapPinOffset;
                annotationView.RemoveSubviews();

                var numberLabel = new UILabel { 
                    TextColor = Theme.MapPinText,
                    Font = Theme.MapPinFont,
                    TextAlignment = UITextAlignment.Center,
                    BackgroundColor = UIColor.Clear,
                    Text = mapAnnotation.Number.ToString(),
                    Frame = new RectangleF(Theme.MapPinTextOffset, new SizeF(25, 25))
                };
                annotationView.AddSubview(numberLabel);

                if (CanShowCallout && CanShowDetails)
                {
                    var detailButton = UIButton.FromType(UIButtonType.DetailDisclosure);
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

                    // TODO: It doesn't work, to find out why
                    /*if (mapAnnotation.Logo != null)
                    {
                        var imageView = new UIImageView();
                        annotationView.LeftCalloutAccessoryView = imageView;

                        _imageHelper = new MvxImageViewLoader(() => imageView);
                        _imageHelper.ImageUrl = mapAnnotation.Logo;
                    }*/
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
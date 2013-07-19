using System.Collections.Generic;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Utils;

namespace SmartWalk.iOS.Views
{
    public class OrgEventMapViewDelegate : MKMapViewDelegate
    {
        private readonly OrgEventViewModel _viewModel;
        private readonly List<MKPinAnnotationView> _viewLinksList = 
            new List<MKPinAnnotationView>(); // to prevent GC

        private MvxImageViewLoader _imageHelper;
        private string _annotationIdentifier = "BasicAnnotation";

        public OrgEventMapViewDelegate(OrgEventViewModel viewModel)
        {
            _viewModel = viewModel;

        }

        public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, NSObject annotation)
        {
            var annotationView = (MKPinAnnotationView)mapView.DequeueReusableAnnotation(_annotationIdentifier);
            if (annotationView == null)
            {
                if (annotation is MKUserLocation)
                {
                    return null;
                }
                else
                {
                    annotationView = new MKPinAnnotationView(annotation, _annotationIdentifier);
                }
            }
            else
            {
                annotationView.Annotation = annotation;
            }

            if (!_viewLinksList.Contains(annotationView))
            {
                _viewLinksList.Add(annotationView);
            }

            var venueAnnotation = annotation as VenueAnnotation;
            if (venueAnnotation != null)
            {
                annotationView.CanShowCallout = true;
                annotationView.AnimatesDrop = true;

                var detailButton = UIButton.FromType(UIButtonType.DetailDisclosure);
                detailButton.TouchUpInside += (s, e) => 
                {
                    if (_viewModel.NavigateVenueCommand.CanExecute(venueAnnotation.Venue))
                    {
                        _viewModel.NavigateVenueCommand.Execute(venueAnnotation.Venue);
                    };
                };

                annotationView.RightCalloutAccessoryView = detailButton;

                if (venueAnnotation.Venue.Info.Logo != null)
                {
                    var imageView = new UIImageView();
                    annotationView.LeftCalloutAccessoryView = imageView;

                    _imageHelper = new MvxImageViewLoader(() => imageView);
                    _imageHelper.ImageUrl = venueAnnotation.Venue.Info.Logo;
                }
            }

            return annotationView;
        }
    }
}
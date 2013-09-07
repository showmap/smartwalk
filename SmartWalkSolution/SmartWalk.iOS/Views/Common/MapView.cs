using System.ComponentModel;
using System.Linq;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.Core.ViewModels;
using SmartWalk.iOS.Utils;

namespace SmartWalk.iOS.Views.Common
{
    public partial class MapView : MvxViewController
    {
        public new MapViewModel ViewModel
        {
            get { return (MapViewModel)base.ViewModel; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

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
                ViewModel.Annotation.AddressInfos != null &&
                ViewModel.Annotation.AddressInfos.Length > 0)
            {
                var annotations = ViewModel.Annotation.AddressInfos
                    .Select(address => new MapViewAnnotation(
                        ViewModel.Annotation.Number,
                        ViewModel.Annotation.Title,
                        address)).ToArray();
                var coordinates = MapUtil.GetAnnotationsCoordinates(annotations);

                MapViewControl.SetRegion(MapUtil.CoordinateRegionForCoordinates(coordinates), false);
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
    }

    public class MapViewAnnotation : MKAnnotation
    {
        private readonly string _title;
        private readonly string _subTitle;

        public MapViewAnnotation(int number, string title, AddressInfo addressInfo)
        {
            _title = MapUtil.GetAnnotationTitle(number, title);
            _subTitle = addressInfo.Address;

            Coordinate = new CLLocationCoordinate2D(
                addressInfo.Latitude,
                addressInfo.Longitude);
        }

        public override CLLocationCoordinate2D Coordinate { get; set; }

        public override string Title
        {
            get
            {
                return _title;
            }
        }

        public override string Subtitle
        {
            get
            {
                return _subTitle;
            }
        }
    }
}
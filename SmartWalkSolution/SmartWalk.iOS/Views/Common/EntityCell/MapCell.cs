using System;
using System.Linq;
using System.Windows.Input;
using MonoTouch.Foundation;
using MonoTouch.MapKit;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Model.Interfaces;
using SmartWalk.Core.Utils;
using SmartWalk.iOS.Utils.Map;
using MonoTouch.CoreLocation;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class MapCell : CollectionCellBase
    {
        public static readonly UINib Nib = UINib.FromName("MapCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("MapCell");

        private UITapGestureRecognizer _mapTapGesture;

        public MapCell(IntPtr handle) : base (handle)
        {
        }

        public new Entity DataContext
        {
            get { return (Entity)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ICommand NavigateAddressesCommand { get; set; }

        public static MapCell Create()
        {
            return (MapCell)Nib.Instantiate(null, null)[0];
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);

            if (newsuper == null)
            {
                NavigateAddressesCommand = null;

                DisposeGestures();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }

        protected override void OnInitialize()
        {
            InitializeGestures();

            MapView.Delegate = new MapDelegate { CanShowCallout = false };
        }

        protected override void OnDataContextChanged()
        {
            var previousAnnotations = MapView.Annotations.Where(a => !(a is MKUserLocation)).ToArray();
            var isFirstLoading = previousAnnotations.Length == 0;
            MapView.RemoveAnnotations(previousAnnotations);

            if (DataContext != null && 
                DataContext.Info.HasAddress())
            {
                var annotations = DataContext.Info.Addresses.Select(
                        a => new MapViewAnnotation(
                            DataContext is INumberEntity ? ((INumberEntity)DataContext).Number : 0,
                                DataContext.Info.Name, 
                                a)).ToArray();
                var coordinates = MapUtil.GetAnnotationsCoordinates(annotations);

                // Adjusting vertical to keep a pin in the cell center
                for (var i = 0; i < coordinates.Length; i++)
                {
                    coordinates[i].Latitude += 0.0002;
                }

                MapView.SetRegion(
                    MapUtil.CoordinateRegionForCoordinates(coordinates, new MKMapSize(2000, 2000)),
                    !isFirstLoading);

                MapView.AddAnnotations(annotations);
            }
        }

        private void InitializeGestures()
        {
            _mapTapGesture = new UITapGestureRecognizer(() => {
                if (NavigateAddressesCommand != null &&
                    NavigateAddressesCommand.CanExecute(DataContext))
                {
                    NavigateAddressesCommand.Execute(DataContext);
                }
            }) {
                NumberOfTouchesRequired = (uint)1,
                NumberOfTapsRequired = (uint)1
            };

            MapView.AddGestureRecognizer(_mapTapGesture);
        }

        private void DisposeGestures()
        {
            if (_mapTapGesture != null)
            {
                MapView.RemoveGestureRecognizer(_mapTapGesture);
                _mapTapGesture.Dispose();
                _mapTapGesture = null;
            }
        }
    }
}
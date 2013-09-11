using System;
using System.Windows.Input;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;
using SmartWalk.iOS.Utils;
using SmartWalk.Core.Model.Interfaces;

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
        }

        protected override void OnDataContextChanged()
        {
            // TODO: check current location annotation
            MapView.RemoveAnnotations(MapView.Annotations);

            if (DataContext != null && 
                DataContext.Info.HasAddress())
            {
                var annotation = new MapViewAnnotation(
                    DataContext is INumberEntity ? ((INumberEntity)DataContext).Number : 0,
                    DataContext.Info.Name, 
                    DataContext.Info.Addresses[0]);
                MapView.SetRegion(
                    MapUtil.CoordinateRegionForCoordinates(annotation.Coordinate), false);
                MapView.AddAnnotation(annotation);
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

        partial void OnNavigateButtonClick(NSObject sender, UIEvent @event)
        {
        }
    }
}
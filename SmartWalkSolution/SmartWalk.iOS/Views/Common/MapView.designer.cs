// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.iOS.Views.Common
{
	[Register ("MapView")]
	partial class MapView
	{
		[Outlet]
		MonoTouch.UIKit.UILabel AddressLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint BottomToolBarHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView BottomToolBarView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton CopyButton { get; set; }

		[Outlet]
		MonoTouch.MapKit.MKMapView MapViewControl { get; set; }

		[Action ("OnCopyButtonClick:forEvent:")]
		partial void OnCopyButtonClick (MonoTouch.UIKit.UIButton sender, MonoTouch.UIKit.UIEvent @event);
		
		void ReleaseDesignerOutlets ()
		{
			if (MapViewControl != null) {
				MapViewControl.Dispose ();
				MapViewControl = null;
			}

			if (AddressLabel != null) {
				AddressLabel.Dispose ();
				AddressLabel = null;
			}

			if (BottomToolBarView != null) {
				BottomToolBarView.Dispose ();
				BottomToolBarView = null;
			}

			if (CopyButton != null) {
				CopyButton.Dispose ();
				CopyButton = null;
			}

			if (BottomToolBarHeightConstraint != null) {
				BottomToolBarHeightConstraint.Dispose ();
				BottomToolBarHeightConstraint = null;
			}
		}
	}
}

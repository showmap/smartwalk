// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
	[Register ("MapCell")]
	partial class MapCell
	{
		[Outlet]
		UIKit.UIView AddressContainer { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint AddressHeightConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel AddressLabel { get; set; }

		[Outlet]
		UIKit.UIView ContentView { get; set; }

		[Outlet]
		UIKit.UIView CoverView { get; set; }

		[Outlet]
		MapKit.MKMapView MapView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ContentView != null) {
				ContentView.Dispose ();
				ContentView = null;
			}

			if (AddressContainer != null) {
				AddressContainer.Dispose ();
				AddressContainer = null;
			}

			if (AddressHeightConstraint != null) {
				AddressHeightConstraint.Dispose ();
				AddressHeightConstraint = null;
			}

			if (AddressLabel != null) {
				AddressLabel.Dispose ();
				AddressLabel = null;
			}

			if (CoverView != null) {
				CoverView.Dispose ();
				CoverView = null;
			}

			if (MapView != null) {
				MapView.Dispose ();
				MapView = null;
			}
		}
	}
}

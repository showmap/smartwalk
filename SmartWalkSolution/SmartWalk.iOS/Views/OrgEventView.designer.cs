// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views
{
	[Register ("OrgEventView")]
	partial class OrgEventView
	{
		[Outlet]
		MonoTouch.UIKit.UIView MapView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView TableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView VenuesAndShowsTableView { get; set; }

		[Outlet]
		MonoTouch.MapKit.MKMapView VenuesMapView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TableView != null) {
				TableView.Dispose ();
				TableView = null;
			}

			if (MapView != null) {
				MapView.Dispose ();
				MapView = null;
			}

			if (VenuesAndShowsTableView != null) {
				VenuesAndShowsTableView.Dispose ();
				VenuesAndShowsTableView = null;
			}

			if (VenuesMapView != null) {
				VenuesMapView.Dispose ();
				VenuesMapView = null;
			}
		}
	}
}

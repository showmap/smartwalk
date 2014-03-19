// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
	[Register ("OrgEventView")]
	partial class OrgEventView
	{
		[Outlet]
		MonoTouch.UIKit.UIView MapPanel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint MapPanelTopConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView ProgressViewContainer { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView TablePanel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint TablePanelTopConstaint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.FixedTableView VenuesAndShowsTableView { get; set; }

		[Outlet]
		MonoTouch.MapKit.MKMapView VenuesMapView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (MapPanel != null) {
				MapPanel.Dispose ();
				MapPanel = null;
			}

			if (ProgressViewContainer != null) {
				ProgressViewContainer.Dispose ();
				ProgressViewContainer = null;
			}

			if (TablePanel != null) {
				TablePanel.Dispose ();
				TablePanel = null;
			}

			if (VenuesAndShowsTableView != null) {
				VenuesAndShowsTableView.Dispose ();
				VenuesAndShowsTableView = null;
			}

			if (VenuesMapView != null) {
				VenuesMapView.Dispose ();
				VenuesMapView = null;
			}

			if (TablePanelTopConstaint != null) {
				TablePanelTopConstaint.Dispose ();
				TablePanelTopConstaint = null;
			}

			if (MapPanelTopConstraint != null) {
				MapPanelTopConstraint.Dispose ();
				MapPanelTopConstraint = null;
			}
		}
	}
}

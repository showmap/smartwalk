// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
	[Register ("OrgEventView")]
	partial class OrgEventView
	{
		[Outlet]
		UIKit.NSLayoutConstraint FullscreenHeightConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint FullscreenWidthConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line MapBottomSeparator { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.ButtonBarButton MapFullscreenButton { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint MapHeightConstraint { get; set; }

		[Outlet]
		UIKit.UIView MapPanel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint MapToTableConstraint { get; set; }

		[Outlet]
		UIKit.UIView ProgressViewContainer { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ProgressViewTopConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TableHeightConstraint { get; set; }

		[Outlet]
        UIKit.UITableView VenuesAndShowsTableView { get; set; }

		[Outlet]
		MapKit.MKMapView VenuesMapView { get; set; }

		[Action ("OnMapFullscreenTouchUpInside:")]
		partial void OnMapFullscreenTouchUpInside (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (FullscreenHeightConstraint != null) {
				FullscreenHeightConstraint.Dispose ();
				FullscreenHeightConstraint = null;
			}

			if (FullscreenWidthConstraint != null) {
				FullscreenWidthConstraint.Dispose ();
				FullscreenWidthConstraint = null;
			}

			if (MapBottomSeparator != null) {
				MapBottomSeparator.Dispose ();
				MapBottomSeparator = null;
			}

			if (MapFullscreenButton != null) {
				MapFullscreenButton.Dispose ();
				MapFullscreenButton = null;
			}

			if (MapHeightConstraint != null) {
				MapHeightConstraint.Dispose ();
				MapHeightConstraint = null;
			}

			if (MapPanel != null) {
				MapPanel.Dispose ();
				MapPanel = null;
			}

			if (MapToTableConstraint != null) {
				MapToTableConstraint.Dispose ();
				MapToTableConstraint = null;
			}

			if (ProgressViewContainer != null) {
				ProgressViewContainer.Dispose ();
				ProgressViewContainer = null;
			}

			if (ProgressViewTopConstraint != null) {
				ProgressViewTopConstraint.Dispose ();
				ProgressViewTopConstraint = null;
			}

			if (TableHeightConstraint != null) {
				TableHeightConstraint.Dispose ();
				TableHeightConstraint = null;
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

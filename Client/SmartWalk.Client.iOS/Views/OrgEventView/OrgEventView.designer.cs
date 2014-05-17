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
		MonoTouch.UIKit.NSLayoutConstraint FullscreenHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint FullscreenWidthConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line MapBottomSeparator { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView MapContentView { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.ButtonBarButton MapFullscreenButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint MapHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView MapPanel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint MapToTableConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton MapTypeButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView ProgressViewContainer { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ProgressViewTopConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint TableHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView TablePanel { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.FixedTableView VenuesAndShowsTableView { get; set; }

		[Outlet]
		MonoTouch.MapKit.MKMapView VenuesMapView { get; set; }

		[Action ("OnMapFullscreenTouchUpInside:")]
		partial void OnMapFullscreenTouchUpInside (MonoTouch.Foundation.NSObject sender);

		[Action ("OnMapTypeTouchUpInside:")]
		partial void OnMapTypeTouchUpInside (MonoTouch.Foundation.NSObject sender);
		
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

			if (MapTypeButton != null) {
				MapTypeButton.Dispose ();
				MapTypeButton = null;
			}

			if (MapBottomSeparator != null) {
				MapBottomSeparator.Dispose ();
				MapBottomSeparator = null;
			}

			if (MapContentView != null) {
				MapContentView.Dispose ();
				MapContentView = null;
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
		}
	}
}

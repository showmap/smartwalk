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
	[Register ("ListSettingsView")]
	partial class ListSettingsView
	{
		[Outlet]
		MonoTouch.UIKit.UIView BackgroundView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ContainerHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ContainerTopConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel GroupByLocationLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISwitch GroupByLocationSwitch { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView PlaceholderView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel SortByLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView SortByPlaceholder { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISegmentedControl SortBySegments { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line TopSeparator { get; set; }

		[Action ("OnGroupByLocationTouchUpInside:forEvent:")]
		partial void OnGroupByLocationTouchUpInside (MonoTouch.UIKit.UISwitch sender, MonoTouch.UIKit.UIEvent @event);

		[Action ("OnSortBySegmentsValueChanged:forEvent:")]
		partial void OnSortBySegmentsValueChanged (MonoTouch.UIKit.UISegmentedControl sender, MonoTouch.UIKit.UIEvent @event);
		
		void ReleaseDesignerOutlets ()
		{
			if (TopSeparator != null) {
				TopSeparator.Dispose ();
				TopSeparator = null;
			}

			if (ContainerHeightConstraint != null) {
				ContainerHeightConstraint.Dispose ();
				ContainerHeightConstraint = null;
			}

			if (ContainerTopConstraint != null) {
				ContainerTopConstraint.Dispose ();
				ContainerTopConstraint = null;
			}

			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}

			if (PlaceholderView != null) {
				PlaceholderView.Dispose ();
				PlaceholderView = null;
			}

			if (GroupByLocationLabel != null) {
				GroupByLocationLabel.Dispose ();
				GroupByLocationLabel = null;
			}

			if (GroupByLocationSwitch != null) {
				GroupByLocationSwitch.Dispose ();
				GroupByLocationSwitch = null;
			}

			if (SortByLabel != null) {
				SortByLabel.Dispose ();
				SortByLabel = null;
			}

			if (SortByPlaceholder != null) {
				SortByPlaceholder.Dispose ();
				SortByPlaceholder = null;
			}

			if (SortBySegments != null) {
				SortBySegments.Dispose ();
				SortBySegments = null;
			}
		}
	}
}

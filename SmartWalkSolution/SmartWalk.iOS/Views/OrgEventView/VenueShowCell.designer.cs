// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views.OrgEventView
{
	[Register ("VenueShowCell")]
	partial class VenueShowCell
	{
		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint DescriptionAndImageSpaceConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint DetailsHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DetailsLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel EndTimeLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint EndTimeLeftSpaceConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint EndTimeRightSpaceConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ImageAndDetailsSpaceConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ImageHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView ImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ImageWidthConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel StartTimeLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DescriptionAndImageSpaceConstraint != null) {
				DescriptionAndImageSpaceConstraint.Dispose ();
				DescriptionAndImageSpaceConstraint = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (DetailsHeightConstraint != null) {
				DetailsHeightConstraint.Dispose ();
				DetailsHeightConstraint = null;
			}

			if (DetailsLabel != null) {
				DetailsLabel.Dispose ();
				DetailsLabel = null;
			}

			if (EndTimeLabel != null) {
				EndTimeLabel.Dispose ();
				EndTimeLabel = null;
			}

			if (EndTimeLeftSpaceConstraint != null) {
				EndTimeLeftSpaceConstraint.Dispose ();
				EndTimeLeftSpaceConstraint = null;
			}

			if (EndTimeRightSpaceConstraint != null) {
				EndTimeRightSpaceConstraint.Dispose ();
				EndTimeRightSpaceConstraint = null;
			}

			if (ImageAndDetailsSpaceConstraint != null) {
				ImageAndDetailsSpaceConstraint.Dispose ();
				ImageAndDetailsSpaceConstraint = null;
			}

			if (ImageHeightConstraint != null) {
				ImageHeightConstraint.Dispose ();
				ImageHeightConstraint = null;
			}

			if (ImageWidthConstraint != null) {
				ImageWidthConstraint.Dispose ();
				ImageWidthConstraint = null;
			}

			if (ImageView != null) {
				ImageView.Dispose ();
				ImageView = null;
			}

			if (StartTimeLabel != null) {
				StartTimeLabel.Dispose ();
				StartTimeLabel = null;
			}
		}
	}
}

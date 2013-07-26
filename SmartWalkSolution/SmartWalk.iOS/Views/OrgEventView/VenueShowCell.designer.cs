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
		MonoTouch.UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint DescriptionLogoSpaceConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel EndTimeLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint LogoDetailsSpaceConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint LogoHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView LogoImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint MoreInfoHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel StartTimeLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (DescriptionLogoSpaceConstraint != null) {
				DescriptionLogoSpaceConstraint.Dispose ();
				DescriptionLogoSpaceConstraint = null;
			}

			if (LogoDetailsSpaceConstraint != null) {
				LogoDetailsSpaceConstraint.Dispose ();
				LogoDetailsSpaceConstraint = null;
			}

			if (EndTimeLabel != null) {
				EndTimeLabel.Dispose ();
				EndTimeLabel = null;
			}

			if (LogoHeightConstraint != null) {
				LogoHeightConstraint.Dispose ();
				LogoHeightConstraint = null;
			}

			if (LogoImageView != null) {
				LogoImageView.Dispose ();
				LogoImageView = null;
			}

			if (MoreInfoHeightConstraint != null) {
				MoreInfoHeightConstraint.Dispose ();
				MoreInfoHeightConstraint = null;
			}

			if (StartTimeLabel != null) {
				StartTimeLabel.Dispose ();
				StartTimeLabel = null;
			}
		}
	}
}

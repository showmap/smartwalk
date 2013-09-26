// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.iOS.Views.OrgEventView
{
	[Register ("VenueShowCell")]
	partial class VenueShowCell
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView ClockImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint DescriptionAndImageSpaceConstraint { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.CopyLabel DescriptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint DescriptionLeftConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint DetailsHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DetailsLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel EndTimeLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ImageAndDetailsSpaceConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ImageHeightConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ImageWidthConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel StartTimeLabel { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.ProgressImageView ThumbImageView { get; set; }
		
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

			if (DescriptionLeftConstraint != null) {
				DescriptionLeftConstraint.Dispose ();
				DescriptionLeftConstraint = null;
			}

			if (DetailsLabel != null) {
				DetailsLabel.Dispose ();
				DetailsLabel = null;
			}

			if (EndTimeLabel != null) {
				EndTimeLabel.Dispose ();
				EndTimeLabel = null;
			}

			if (ImageAndDetailsSpaceConstraint != null) {
				ImageAndDetailsSpaceConstraint.Dispose ();
				ImageAndDetailsSpaceConstraint = null;
			}

			if (ImageHeightConstraint != null) {
				ImageHeightConstraint.Dispose ();
				ImageHeightConstraint = null;
			}

			if (ClockImageView != null) {
				ClockImageView.Dispose ();
				ClockImageView = null;
			}

			if (ImageWidthConstraint != null) {
				ImageWidthConstraint.Dispose ();
				ImageWidthConstraint = null;
			}

			if (StartTimeLabel != null) {
				StartTimeLabel.Dispose ();
				StartTimeLabel = null;
			}

			if (ThumbImageView != null) {
				ThumbImageView.Dispose ();
				ThumbImageView = null;
			}
		}
	}
}

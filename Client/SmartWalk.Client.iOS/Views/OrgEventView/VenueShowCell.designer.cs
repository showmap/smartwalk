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
	[Register ("VenueShowCell")]
	partial class VenueShowCell
	{
		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint DescriptionAndImageSpaceConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel DescriptionLabel { get; set; }

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
		SmartWalk.Client.iOS.Controls.Line Separator { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel StartTimeLabel { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.ProgressImageView ThumbImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView TimeBackgroundView { get; set; }
		
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

			if (DescriptionLeftConstraint != null) {
				DescriptionLeftConstraint.Dispose ();
				DescriptionLeftConstraint = null;
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

			if (Separator != null) {
				Separator.Dispose ();
				Separator = null;
			}

			if (StartTimeLabel != null) {
				StartTimeLabel.Dispose ();
				StartTimeLabel = null;
			}

			if (ThumbImageView != null) {
				ThumbImageView.Dispose ();
				ThumbImageView = null;
			}

			if (TimeBackgroundView != null) {
				TimeBackgroundView.Dispose ();
				TimeBackgroundView = null;
			}
		}
	}
}

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
	[Register ("VenueShowCell")]
	partial class VenueShowCell
	{
		[Outlet]
		UIKit.NSLayoutConstraint DescriptionAndImageSpaceConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel DescriptionLabel { get; set; }

		[Outlet]
		UIKit.UIButton DetailsButton { get; set; }

		[Outlet]
		UIKit.UILabel DetailsLabel { get; set; }

		[Outlet]
		UIKit.UILabel EndTimeLabel { get; set; }

		[Outlet]
		UIKit.UIView HeaderContainer { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint HeaderHeightConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ImageAndDetailsSpaceConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ImageHeightConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ImageWidthConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line Separator { get; set; }

		[Outlet]
		UIKit.UILabel StartTimeLabel { get; set; }

		[Outlet]
		UIKit.UIView SubHeaderContainer { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint SubHeaderHeightConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.ProgressImageView ThumbImageView { get; set; }

		[Outlet]
		UIKit.UIView TimeBackgroundView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TimeTopConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TitleAndDescriptionSpaceConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel TitleLabel { get; set; }

		[Action ("OnDetailsButtonClick:")]
		partial void OnDetailsButtonClick (Foundation.NSObject sender);
		
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

			if (DetailsLabel != null) {
				DetailsLabel.Dispose ();
				DetailsLabel = null;
			}

			if (EndTimeLabel != null) {
				EndTimeLabel.Dispose ();
				EndTimeLabel = null;
			}

			if (HeaderContainer != null) {
				HeaderContainer.Dispose ();
				HeaderContainer = null;
			}

			if (HeaderHeightConstraint != null) {
				HeaderHeightConstraint.Dispose ();
				HeaderHeightConstraint = null;
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

			if (SubHeaderContainer != null) {
				SubHeaderContainer.Dispose ();
				SubHeaderContainer = null;
			}

			if (SubHeaderHeightConstraint != null) {
				SubHeaderHeightConstraint.Dispose ();
				SubHeaderHeightConstraint = null;
			}

			if (ThumbImageView != null) {
				ThumbImageView.Dispose ();
				ThumbImageView = null;
			}

			if (TimeBackgroundView != null) {
				TimeBackgroundView.Dispose ();
				TimeBackgroundView = null;
			}

			if (TimeTopConstraint != null) {
				TimeTopConstraint.Dispose ();
				TimeTopConstraint = null;
			}

			if (TitleAndDescriptionSpaceConstraint != null) {
				TitleAndDescriptionSpaceConstraint.Dispose ();
				TitleAndDescriptionSpaceConstraint = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (DetailsButton != null) {
				DetailsButton.Dispose ();
				DetailsButton = null;
			}
		}
	}
}

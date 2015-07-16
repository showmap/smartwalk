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
		UIKit.NSLayoutConstraint BottomBorderLeftConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint DescriptionAndDetailsSpaceConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel DescriptionLabel { get; set; }

		[Outlet]
		UIKit.UIButton DetailsButton { get; set; }

		[Outlet]
		UIKit.UILabel DetailsLabel { get; set; }

		[Outlet]
		UIKit.UILabel EndTimeLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint LocationAndDescriptionConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel LocationLabel { get; set; }

		[Outlet]
		UIKit.UIButton NavigateOnMapButton { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line Separator { get; set; }

		[Outlet]
		UIKit.UIButton StarButton { get; set; }

		[Outlet]
		UIKit.UILabel StartTimeLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ThumbHeightConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.ProgressImageView ThumbImageView { get; set; }

		[Outlet]
		UIKit.UILabel ThumbLabel { get; set; }

		[Outlet]
		UIKit.UIView ThumbLabelView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ThumbLeftConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ThumbTopConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ThumbWidthConstraint { get; set; }

		[Outlet]
		UIKit.UIView TimeBackgroundView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TimeTopConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TitleAndLocationConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel TitleLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TitleLeftConstraint { get; set; }

		[Action ("OnDetailsButtonClick:")]
		partial void OnDetailsButtonClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BottomBorderLeftConstraint != null) {
				BottomBorderLeftConstraint.Dispose ();
				BottomBorderLeftConstraint = null;
			}

			if (DescriptionAndDetailsSpaceConstraint != null) {
				DescriptionAndDetailsSpaceConstraint.Dispose ();
				DescriptionAndDetailsSpaceConstraint = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (DetailsButton != null) {
				DetailsButton.Dispose ();
				DetailsButton = null;
			}

			if (DetailsLabel != null) {
				DetailsLabel.Dispose ();
				DetailsLabel = null;
			}

			if (EndTimeLabel != null) {
				EndTimeLabel.Dispose ();
				EndTimeLabel = null;
			}

			if (LocationAndDescriptionConstraint != null) {
				LocationAndDescriptionConstraint.Dispose ();
				LocationAndDescriptionConstraint = null;
			}

			if (LocationLabel != null) {
				LocationLabel.Dispose ();
				LocationLabel = null;
			}

			if (NavigateOnMapButton != null) {
				NavigateOnMapButton.Dispose ();
				NavigateOnMapButton = null;
			}

			if (Separator != null) {
				Separator.Dispose ();
				Separator = null;
			}

			if (StartTimeLabel != null) {
				StartTimeLabel.Dispose ();
				StartTimeLabel = null;
			}

			if (ThumbHeightConstraint != null) {
				ThumbHeightConstraint.Dispose ();
				ThumbHeightConstraint = null;
			}

			if (ThumbImageView != null) {
				ThumbImageView.Dispose ();
				ThumbImageView = null;
			}

			if (ThumbLabel != null) {
				ThumbLabel.Dispose ();
				ThumbLabel = null;
			}

			if (ThumbLabelView != null) {
				ThumbLabelView.Dispose ();
				ThumbLabelView = null;
			}

			if (ThumbLeftConstraint != null) {
				ThumbLeftConstraint.Dispose ();
				ThumbLeftConstraint = null;
			}

			if (ThumbTopConstraint != null) {
				ThumbTopConstraint.Dispose ();
				ThumbTopConstraint = null;
			}

			if (ThumbWidthConstraint != null) {
				ThumbWidthConstraint.Dispose ();
				ThumbWidthConstraint = null;
			}

			if (TimeBackgroundView != null) {
				TimeBackgroundView.Dispose ();
				TimeBackgroundView = null;
			}

			if (TimeTopConstraint != null) {
				TimeTopConstraint.Dispose ();
				TimeTopConstraint = null;
			}

			if (TitleAndLocationConstraint != null) {
				TitleAndLocationConstraint.Dispose ();
				TitleAndLocationConstraint = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TitleLeftConstraint != null) {
				TitleLeftConstraint.Dispose ();
				TitleLeftConstraint = null;
			}

			if (StarButton != null) {
				StarButton.Dispose ();
				StarButton = null;
			}
		}
	}
}

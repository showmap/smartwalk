// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.Common
{
	[Register ("ImageBackgroundView")]
	partial class ImageBackgroundView
	{
		[Outlet]
		UIKit.UIImageView BackgroundImage { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView ProgressView { get; set; }

		[Outlet]
        UIKit.UILabel SubtitleLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TitleBottomGapConstraint { get; set; }

		[Outlet]
        UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BackgroundImage != null) {
				BackgroundImage.Dispose ();
				BackgroundImage = null;
			}

			if (ProgressView != null) {
				ProgressView.Dispose ();
				ProgressView = null;
			}

			if (SubtitleLabel != null) {
				SubtitleLabel.Dispose ();
				SubtitleLabel = null;
			}

			if (TitleBottomGapConstraint != null) {
				TitleBottomGapConstraint.Dispose ();
				TitleBottomGapConstraint = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}
		}
	}
}

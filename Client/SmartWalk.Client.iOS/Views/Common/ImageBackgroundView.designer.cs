// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.Common
{
	[Register ("ImageBackgroundView")]
	partial class ImageBackgroundView
	{
		[Outlet]
		SmartWalk.Client.iOS.Controls.ProgressImageView BackgroundImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView GradientPlaceholder { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton SubtitleButton { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel SubtitleLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint SubtitleLeftConstraint { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint TitleBottomConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel TitleLabel { get; set; }

		[Action ("OnSubtitleButtonTouchInsideUp:")]
		partial void OnSubtitleButtonTouchInsideUp (MonoTouch.UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BackgroundImage != null) {
				BackgroundImage.Dispose ();
				BackgroundImage = null;
			}

			if (GradientPlaceholder != null) {
				GradientPlaceholder.Dispose ();
				GradientPlaceholder = null;
			}

			if (SubtitleButton != null) {
				SubtitleButton.Dispose ();
				SubtitleButton = null;
			}

			if (SubtitleLabel != null) {
				SubtitleLabel.Dispose ();
				SubtitleLabel = null;
			}

			if (SubtitleLeftConstraint != null) {
				SubtitleLeftConstraint.Dispose ();
				SubtitleLeftConstraint = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TitleBottomConstraint != null) {
				TitleBottomConstraint.Dispose ();
				TitleBottomConstraint = null;
			}
		}
	}
}

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
	[Register ("ImageBackgroundSimpleView")]
	partial class ImageBackgroundSimpleView
	{
		[Outlet]
		SmartWalk.Client.iOS.Controls.ProgressImageView BackgroundImage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView GradientPlaceholder { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint SubtitleHeightConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel SubtitleLabel { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel TitleLabel { get; set; }
		
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

			if (SubtitleHeightConstraint != null) {
				SubtitleHeightConstraint.Dispose ();
				SubtitleHeightConstraint = null;
			}

			if (SubtitleLabel != null) {
				SubtitleLabel.Dispose ();
				SubtitleLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}
		}
	}
}

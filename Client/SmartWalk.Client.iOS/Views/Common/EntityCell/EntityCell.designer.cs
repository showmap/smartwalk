// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
	[Register ("EntityCell")]
	partial class EntityCell
	{
		[Outlet]
		MonoTouch.UIKit.UIView BottomGradientView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint DescriptionBottomConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel DescriptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint DescriptionTopConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Placeholder ImagePlaceholder { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line PlaceholderSeparator { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line Separator { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BottomGradientView != null) {
				BottomGradientView.Dispose ();
				BottomGradientView = null;
			}

			if (DescriptionBottomConstraint != null) {
				DescriptionBottomConstraint.Dispose ();
				DescriptionBottomConstraint = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (DescriptionTopConstraint != null) {
				DescriptionTopConstraint.Dispose ();
				DescriptionTopConstraint = null;
			}

			if (ImagePlaceholder != null) {
				ImagePlaceholder.Dispose ();
				ImagePlaceholder = null;
			}

			if (PlaceholderSeparator != null) {
				PlaceholderSeparator.Dispose ();
				PlaceholderSeparator = null;
			}

			if (Separator != null) {
				Separator.Dispose ();
				Separator = null;
			}
		}
	}
}

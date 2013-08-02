// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
	[Register ("EntityCell")]
	partial class EntityCell
	{
		[Outlet]
		MonoTouch.UIKit.UIView BottomGradientView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint DescriptionTopSpaceConstraint { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.PagedScrollView ScrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ScrollViewHeightConstraint { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BottomGradientView != null) {
				BottomGradientView.Dispose ();
				BottomGradientView = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (DescriptionTopSpaceConstraint != null) {
				DescriptionTopSpaceConstraint.Dispose ();
				DescriptionTopSpaceConstraint = null;
			}

			if (ScrollView != null) {
				ScrollView.Dispose ();
				ScrollView = null;
			}

			if (ScrollViewHeightConstraint != null) {
				ScrollViewHeightConstraint.Dispose ();
				ScrollViewHeightConstraint = null;
			}
		}
	}
}

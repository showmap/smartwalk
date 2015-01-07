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
	[Register ("ContactCell")]
	partial class ContactCell
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView ContactIcon { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel ContactLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint ContactVerticalConstraint { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Circle IconView { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.CopyLabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ContactIcon != null) {
				ContactIcon.Dispose ();
				ContactIcon = null;
			}

			if (ContactLabel != null) {
				ContactLabel.Dispose ();
				ContactLabel = null;
			}

			if (IconView != null) {
				IconView.Dispose ();
				IconView = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (ContactVerticalConstraint != null) {
				ContactVerticalConstraint.Dispose ();
				ContactVerticalConstraint = null;
			}
		}
	}
}

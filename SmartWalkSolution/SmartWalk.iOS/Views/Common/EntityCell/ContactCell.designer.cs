// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
	[Register ("ContactCell")]
	partial class ContactCell
	{
		[Outlet]
		MonoTouch.UIKit.UIImageView ContactIcon { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.CopyLabel ContactLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint TitleHeightConstraint { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.CopyLabel TitleLabel { get; set; }
		
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

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TitleHeightConstraint != null) {
				TitleHeightConstraint.Dispose ();
				TitleHeightConstraint = null;
			}
		}
	}
}

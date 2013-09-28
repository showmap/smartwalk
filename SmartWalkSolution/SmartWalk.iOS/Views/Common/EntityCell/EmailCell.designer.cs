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
	[Register ("EmailCell")]
	partial class EmailCell
	{
		[Outlet]
		SmartWalk.iOS.Controls.CopyLabel ContactNameLabel { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.CopyLabel EmailLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView LogoImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint NameHeightConstraint { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ContactNameLabel != null) {
				ContactNameLabel.Dispose ();
				ContactNameLabel = null;
			}

			if (EmailLabel != null) {
				EmailLabel.Dispose ();
				EmailLabel = null;
			}

			if (NameHeightConstraint != null) {
				NameHeightConstraint.Dispose ();
				NameHeightConstraint = null;
			}

			if (LogoImageView != null) {
				LogoImageView.Dispose ();
				LogoImageView = null;
			}
		}
	}
}

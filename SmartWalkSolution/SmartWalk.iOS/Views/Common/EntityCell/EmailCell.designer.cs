// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
	[Register ("EmailCell")]
	partial class EmailCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel ContactNameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel EmailLabel { get; set; }

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
		}
	}
}

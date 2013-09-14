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
	[Register ("PhoneCell")]
	partial class PhoneCell
	{
		[Outlet]
		SmartWalk.iOS.Controls.CopyLabel ContactNameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint NameHeightConstraint { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.CopyLabel PhoneNumberLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ContactNameLabel != null) {
				ContactNameLabel.Dispose ();
				ContactNameLabel = null;
			}

			if (NameHeightConstraint != null) {
				NameHeightConstraint.Dispose ();
				NameHeightConstraint = null;
			}

			if (PhoneNumberLabel != null) {
				PhoneNumberLabel.Dispose ();
				PhoneNumberLabel = null;
			}
		}
	}
}

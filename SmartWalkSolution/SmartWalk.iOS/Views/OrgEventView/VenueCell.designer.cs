// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.iOS.Views.OrgEventView
{
	[Register ("VenueCell")]
	partial class VenueCell
	{
		[Outlet]
		SmartWalk.iOS.Controls.CopyLabel AddressLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView LogoImageView { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.CopyLabel NameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.NSLayoutConstraint NameLeftConstraint { get; set; }

		[Outlet]
		SmartWalk.iOS.Controls.Shadow Shadow { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (AddressLabel != null) {
				AddressLabel.Dispose ();
				AddressLabel = null;
			}

			if (LogoImageView != null) {
				LogoImageView.Dispose ();
				LogoImageView = null;
			}

			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}

			if (NameLeftConstraint != null) {
				NameLeftConstraint.Dispose ();
				NameLeftConstraint = null;
			}

			if (Shadow != null) {
				Shadow.Dispose ();
				Shadow = null;
			}
		}
	}
}

// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views.Cells
{
	[Register ("EntityCell")]
	partial class EntityCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView LogoImageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel NameLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}

			if (LogoImageView != null) {
				LogoImageView.Dispose ();
				LogoImageView = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}
		}
	}
}

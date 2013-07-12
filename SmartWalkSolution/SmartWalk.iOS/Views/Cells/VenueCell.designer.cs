// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views.Cells
{
	[Register ("VenueCell")]
	partial class VenueCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel NameLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel NumberLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (NumberLabel != null) {
				NumberLabel.Dispose ();
				NumberLabel = null;
			}

			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}
		}
	}
}

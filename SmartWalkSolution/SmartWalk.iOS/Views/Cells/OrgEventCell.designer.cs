// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views.Cells
{
	[Register ("OrgEventCell")]
	partial class OrgEventCell
	{
		[Outlet]
		MonoTouch.UIKit.UILabel DayLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel MonthLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DayLabel != null) {
				DayLabel.Dispose ();
				DayLabel = null;
			}

			if (MonthLabel != null) {
				MonthLabel.Dispose ();
				MonthLabel = null;
			}
		}
	}
}

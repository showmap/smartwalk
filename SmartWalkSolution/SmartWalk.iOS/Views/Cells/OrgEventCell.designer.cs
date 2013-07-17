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
		MonoTouch.UIKit.UILabel DateLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DayLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel HintLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel WeekDayLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DateLabel != null) {
				DateLabel.Dispose ();
				DateLabel = null;
			}

			if (DayLabel != null) {
				DayLabel.Dispose ();
				DayLabel = null;
			}

			if (WeekDayLabel != null) {
				WeekDayLabel.Dispose ();
				WeekDayLabel = null;
			}

			if (HintLabel != null) {
				HintLabel.Dispose ();
				HintLabel = null;
			}
		}
	}
}

// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.OrgView
{
	[Register ("OrgEventCell")]
	partial class OrgEventCell
	{
		[Outlet]
		MonoTouch.UIKit.UIView CalendarView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DateLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel DayLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel HintLabel { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line Separator { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel WeekDayLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CalendarView != null) {
				CalendarView.Dispose ();
				CalendarView = null;
			}

			if (DateLabel != null) {
				DateLabel.Dispose ();
				DateLabel = null;
			}

			if (DayLabel != null) {
				DayLabel.Dispose ();
				DayLabel = null;
			}

			if (HintLabel != null) {
				HintLabel.Dispose ();
				HintLabel = null;
			}

			if (Separator != null) {
				Separator.Dispose ();
				Separator = null;
			}

			if (WeekDayLabel != null) {
				WeekDayLabel.Dispose ();
				WeekDayLabel = null;
			}
		}
	}
}

// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.OrgView
{
	[Register ("OrgEventCell")]
	partial class OrgEventCell
	{
		[Outlet]
		SmartWalk.Client.iOS.Controls.Circle CalendarView { get; set; }

		[Outlet]
		UIKit.UILabel DayLabel { get; set; }

		[Outlet]
		UIKit.UILabel EventTitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel MonthLabel { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line Separator { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CalendarView != null) {
				CalendarView.Dispose ();
				CalendarView = null;
			}

			if (DayLabel != null) {
				DayLabel.Dispose ();
				DayLabel = null;
			}

			if (EventTitleLabel != null) {
				EventTitleLabel.Dispose ();
				EventTitleLabel = null;
			}

			if (MonthLabel != null) {
				MonthLabel.Dispose ();
				MonthLabel = null;
			}

			if (Separator != null) {
				Separator.Dispose ();
				Separator = null;
			}
		}
	}
}

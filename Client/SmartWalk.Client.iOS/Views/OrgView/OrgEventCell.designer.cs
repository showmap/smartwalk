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
		MonoTouch.UIKit.UILabel DayLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel EventTitleLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel MonthLabel { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line Separator { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CalendarView != null) {
				CalendarView.Dispose ();
				CalendarView = null;
			}

			if (Separator != null) {
				Separator.Dispose ();
				Separator = null;
			}

			if (DayLabel != null) {
				DayLabel.Dispose ();
				DayLabel = null;
			}

			if (MonthLabel != null) {
				MonthLabel.Dispose ();
				MonthLabel = null;
			}

			if (EventTitleLabel != null) {
				EventTitleLabel.Dispose ();
				EventTitleLabel = null;
			}
		}
	}
}

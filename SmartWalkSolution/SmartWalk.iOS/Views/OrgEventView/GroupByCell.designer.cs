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
	[Register ("GroupByCell")]
	partial class GroupByCell
	{
		[Outlet]
		MonoTouch.UIKit.UISwitch GroupBySwitch { get; set; }

		[Action ("OnGroupBySwitchTouchUpInside:forEvent:")]
		partial void OnGroupBySwitchTouchUpInside (MonoTouch.UIKit.UISwitch sender, MonoTouch.UIKit.UIEvent @event);
		
		void ReleaseDesignerOutlets ()
		{
			if (GroupBySwitch != null) {
				GroupBySwitch.Dispose ();
				GroupBySwitch = null;
			}
		}
	}
}

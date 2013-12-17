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
	[Register ("OrgEventHeaderView")]
	partial class OrgEventHeaderView
	{
		[Outlet]
		MonoTouch.UIKit.UISwitch GroupByLocationSwitch { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISearchBar SearchBar { get; set; }

		[Action ("OnGroupByLocationTouchUpInside:forEvent:")]
		partial void OnGroupByLocationTouchUpInside (MonoTouch.UIKit.UISwitch sender, MonoTouch.UIKit.UIEvent @event);
		
		void ReleaseDesignerOutlets ()
		{
			if (SearchBar != null) {
				SearchBar.Dispose ();
				SearchBar = null;
			}

			if (GroupByLocationSwitch != null) {
				GroupByLocationSwitch.Dispose ();
				GroupByLocationSwitch = null;
			}
		}
	}
}

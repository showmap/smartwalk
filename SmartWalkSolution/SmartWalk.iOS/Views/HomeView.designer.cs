// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Views
{
	[Register ("HomeView")]
	partial class HomeView
	{
		[Outlet]
		MonoTouch.UIKit.UILabel HelloWorldsLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton PushMeButton { get; set; }

		[Action ("OnPushMeButtonClick:forEvent:")]
		partial void OnPushMeButtonClick (MonoTouch.UIKit.UIButton sender, MonoTouch.UIKit.UIEvent @event);
		
		void ReleaseDesignerOutlets ()
		{
			if (HelloWorldsLabel != null) {
				HelloWorldsLabel.Dispose ();
				HelloWorldsLabel = null;
			}

			if (PushMeButton != null) {
				PushMeButton.Dispose ();
				PushMeButton = null;
			}
		}
	}
}

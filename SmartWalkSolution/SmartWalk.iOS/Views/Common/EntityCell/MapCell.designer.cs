// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
	[Register ("MapCell")]
	partial class MapCell
	{
		[Outlet]
		MonoTouch.MapKit.MKMapView MapView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton NavigateButton { get; set; }

		[Action ("OnNavigateButtonClick:forEvent:")]
		partial void OnNavigateButtonClick (MonoTouch.Foundation.NSObject sender, MonoTouch.UIKit.UIEvent @event);
		
		void ReleaseDesignerOutlets ()
		{
			if (MapView != null) {
				MapView.Dispose ();
				MapView = null;
			}

			if (NavigateButton != null) {
				NavigateButton.Dispose ();
				NavigateButton = null;
			}
		}
	}
}

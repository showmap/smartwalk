// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.Common
{
	[Register ("MapView")]
	partial class MapView
	{
		[Outlet]
		MonoTouch.MapKit.MKMapView MapViewControl { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (MapViewControl != null) {
				MapViewControl.Dispose ();
				MapViewControl = null;
			}
		}
	}
}

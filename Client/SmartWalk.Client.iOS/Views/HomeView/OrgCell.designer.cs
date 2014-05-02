// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.HomeView
{
	[Register ("OrgCell")]
	partial class OrgCell
	{
		[Outlet]
		SmartWalk.Client.iOS.Controls.Placeholder Placeholder { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Placeholder != null) {
				Placeholder.Dispose ();
				Placeholder = null;
			}
		}
	}
}

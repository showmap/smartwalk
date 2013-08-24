// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.iOS.Views.Common
{
	[Register ("ProgressView")]
	partial class ProgressView
	{
		[Outlet]
		MonoTouch.UIKit.UILabel ProgressLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ProgressLabel != null) {
				ProgressLabel.Dispose ();
				ProgressLabel = null;
			}
		}
	}
}

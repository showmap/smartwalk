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
	[Register ("GroupHeaderView")]
	partial class GroupHeaderView
	{
		[Outlet]
		SmartWalk.Client.iOS.Controls.Line BottomSeparator { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel TitleLabel { get; set; }

		[Outlet]
		SmartWalk.Client.iOS.Controls.Line TopSeparator { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BottomSeparator != null) {
				BottomSeparator.Dispose ();
				BottomSeparator = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TopSeparator != null) {
				TopSeparator.Dispose ();
				TopSeparator = null;
			}
		}
	}
}

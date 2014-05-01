// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.OrgEventView
{
	[Register ("OrgEventHeaderView")]
	partial class OrgEventHeaderView
	{
		[Outlet]
		MonoTouch.UIKit.UIButton OptionsButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISearchBar SearchBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel TitleLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView TitlePlaceholder { get; set; }

		[Action ("OnOptionsButtonTouchUpInside:")]
		partial void OnOptionsButtonTouchUpInside (MonoTouch.UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (OptionsButton != null) {
				OptionsButton.Dispose ();
				OptionsButton = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TitlePlaceholder != null) {
				TitlePlaceholder.Dispose ();
				TitlePlaceholder = null;
			}

			if (SearchBar != null) {
				SearchBar.Dispose ();
				SearchBar = null;
			}
		}
	}
}

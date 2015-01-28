// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.Common
{
	[Register ("BrowserView")]
	partial class BrowserView
	{
		[Outlet]
		UIKit.UIBarButtonItem BackButton { get; set; }

		[Outlet]
        UIKit.UIToolbar BottomToolbar { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem ForwardButton { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem LeftSpacer { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem ProgressButton { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem RefreshButton { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint ToolBarHeightConstraint { get; set; }

		[Outlet]
		UIKit.UIWebView WebView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BackButton != null) {
				BackButton.Dispose ();
				BackButton = null;
			}

			if (BottomToolbar != null) {
				BottomToolbar.Dispose ();
				BottomToolbar = null;
			}

			if (ForwardButton != null) {
				ForwardButton.Dispose ();
				ForwardButton = null;
			}

			if (LeftSpacer != null) {
				LeftSpacer.Dispose ();
				LeftSpacer = null;
			}

			if (ProgressButton != null) {
				ProgressButton.Dispose ();
				ProgressButton = null;
			}

			if (RefreshButton != null) {
				RefreshButton.Dispose ();
				RefreshButton = null;
			}

			if (WebView != null) {
				WebView.Dispose ();
				WebView = null;
			}

			if (ToolBarHeightConstraint != null) {
				ToolBarHeightConstraint.Dispose ();
				ToolBarHeightConstraint = null;
			}
		}
	}
}

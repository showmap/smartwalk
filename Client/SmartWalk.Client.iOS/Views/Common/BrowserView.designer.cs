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
		UIKit.NSLayoutConstraint TopConstraint { get; set; }

		[Outlet]
		UIKit.UIWebView WebView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TopConstraint != null) {
				TopConstraint.Dispose ();
				TopConstraint = null;
			}

			if (WebView != null) {
				WebView.Dispose ();
				WebView = null;
			}
		}
	}
}

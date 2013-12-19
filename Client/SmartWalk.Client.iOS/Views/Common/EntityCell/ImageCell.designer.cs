// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace SmartWalk.Client.iOS.Views.Common.EntityCell
{
    [Register ("ImageCell")]
    partial class ImageCell
    {
        [Outlet]
        SmartWalk.Client.iOS.Controls.ProgressImageView ImageView { get; set; }

        [Outlet]
        MonoTouch.UIKit.UIImageView ShadowImageView { get; set; }
        
        void ReleaseDesignerOutlets ()
        {
            if (ImageView != null) {
                ImageView.Dispose ();
                ImageView = null;
            }

            if (ShadowImageView != null) {
                ShadowImageView.Dispose ();
                ShadowImageView = null;
            }
        }
    }
}

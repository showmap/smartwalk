using System;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class WebSiteCell : MvxCollectionViewCell
    {
        public static readonly UINib Nib = UINib.FromName("WebSiteCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("WebSiteCell");

        public WebSiteCell(IntPtr handle) : base(handle)
        {
            Layer.BorderColor = UIColor.Gray.CGColor;
            Layer.CornerRadius = 8;

            this.DelayBind(() => {
                var set = this.CreateBindingSet<WebSiteCell, ContactWebSiteInfo>();
                set.Bind(WebSiteLabel).To(info => info.URL);
                set.Apply();
            });
        }

        public static WebSiteCell Create()
        {
            return (WebSiteCell)Nib.Instantiate(null, null)[0];
        }
    }
}
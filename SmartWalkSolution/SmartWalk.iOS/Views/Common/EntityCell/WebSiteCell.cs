using System;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Converters;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class WebSiteCell : MvxCollectionViewCell
    {
        public static readonly UINib Nib = UINib.FromName("WebSiteCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("WebSiteCell");

        public WebSiteCell(IntPtr handle) : base(handle)
        {
            //Layer.BorderColor = UIColor.Gray.CGColor;
            //Layer.BorderWidth = 1;
            Layer.CornerRadius = 3;

            this.DelayBind(() => {
                var set = this.CreateBindingSet<WebSiteCell, WebSiteInfo>();
                set.Bind(WebSiteLabel).To(info => info)
                    .WithConversion(new ValueConverter<WebSiteInfo>(
                        wsi => wsi.Label ?? wsi.URL), null);
                set.Apply();
            });
        }

        public static WebSiteCell Create()
        {
            return (WebSiteCell)Nib.Instantiate(null, null)[0];
        }
    }
}
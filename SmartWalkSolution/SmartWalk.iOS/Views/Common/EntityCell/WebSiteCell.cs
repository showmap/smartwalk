using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class WebSiteCell : MvxCollectionViewCell
    {
        public static readonly UINib Nib = UINib.FromName("WebSiteCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("WebSiteCell");

        private static readonly string Bindings = String.Format(
            "{0} {1}",
            Reflect<WebSiteCell>.GetProperty(p => p.URLText).Name,
            ReflectExtensions.GetPath<ContactWebSiteInfo>(p => p.URL));

        public WebSiteCell(IntPtr handle) : base(Bindings, handle)
        {
            Layer.BorderColor = UIColor.Gray.CGColor;
            Layer.CornerRadius = 8;
        }

        public string URLText {
            get { return WebSiteLabel.Text; }
            set { WebSiteLabel.Text = value; }
        }

        public static WebSiteCell Create()
        {
            return (WebSiteCell)Nib.Instantiate(null, null)[0];
        }
    }
}
using System;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Converters;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class EmailCell : MvxCollectionViewCell
    {
        public static readonly UINib Nib = UINib.FromName("EmailCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EmailCell");

        public EmailCell(IntPtr handle) : base(handle)
        {
            Layer.BorderColor = UIColor.Gray.CGColor;
            Layer.BorderWidth = 1;
            Layer.CornerRadius = 8;

            this.DelayBind(() => {
                var set = this.CreateBindingSet<EmailCell, EmailInfo>();
                set.Bind(ContactNameLabel).To(info => info.Name);
                set.Bind(EmailLabel).To(info => info.Email);
                set.Bind(NameHeightConstraint).For(p => p.Constant).To(info => info.Name)
                    .WithConversion(new ValueConverter<string>(s => s != null ? 20 : 0), null);
                set.Apply();
            });
        }
        public static EmailCell Create()
        {
            return (EmailCell)Nib.Instantiate(null, null)[0];
        }
    }
}
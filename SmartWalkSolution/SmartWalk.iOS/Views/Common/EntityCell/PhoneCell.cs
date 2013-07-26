using System;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Converters;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class PhoneCell : MvxCollectionViewCell
    {
        public static readonly UINib Nib = UINib.FromName("PhoneCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("PhoneCell");

        public PhoneCell(IntPtr handle) : base(handle)
        {
            //Layer.BorderColor = UIColor.Gray.CGColor;
            //Layer.BorderWidth = 1;
            Layer.CornerRadius = 3;

            this.DelayBind(() => {
                var set = this.CreateBindingSet<PhoneCell, PhoneInfo>();
                set.Bind(ContactNameLabel).To(info => info.Name);
                set.Bind(PhoneNumberLabel).To(info => info.Phone);
                set.Bind(NameHeightConstraint).For(p => p.Constant).To(info => info.Name)
                    .WithConversion(new ValueConverter<string>(s => s != null ? 20 : 0), null);
                set.Apply();
            });
        }

        public static PhoneCell Create()
        {
            return (PhoneCell)Nib.Instantiate(null, null)[0];
        }
    }
}
using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class PhoneCell : MvxCollectionViewCell
    {
        public static readonly UINib Nib = UINib.FromName("PhoneCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("PhoneCell");

        private static readonly string Bindings = String.Format(
            "{0} {1}; {2} {3};",
            Reflect<PhoneCell>.GetProperty(p => p.NameText).Name,
            ReflectExtensions.GetPath<ContactPhoneInfo>(p => p.Name),
            Reflect<PhoneCell>.GetProperty(p => p.PhoneText).Name,
            ReflectExtensions.GetPath<ContactPhoneInfo>(p => p.Phone));

        public PhoneCell(IntPtr handle) : base(Bindings, handle)
        {
            Layer.BorderColor = UIColor.Gray.CGColor;
            Layer.CornerRadius = 8;
        }

        public string NameText {
            get { return ContactNameLabel.Text; }
            set
            { 
                ContactNameLabel.Text = value; 
                NameHeightConstraint.Constant = value != null ? 20 : 0;
            }
        }

        public string PhoneText {
            get { return PhoneNumberLabel.Text; }
            set { PhoneNumberLabel.Text = value; }
        }

        public static PhoneCell Create()
        {
            return (PhoneCell)Nib.Instantiate(null, null)[0];
        }
    }
}
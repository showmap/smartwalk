using System;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.Model;
using SmartWalk.Core.Utils;

namespace SmartWalk.iOS.Views.Common.EntityCell
{
    public partial class EmailCell : MvxCollectionViewCell
    {
        public static readonly UINib Nib = UINib.FromName("EmailCell", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("EmailCell");

        private static readonly string Bindings = String.Format(
            "{0} {1}; {2} {3};",
            Reflect<EmailCell>.GetProperty(p => p.NameText).Name,
            ReflectExtensions.GetPath<ContactEmailInfo>(p => p.Name),
            Reflect<EmailCell>.GetProperty(p => p.EmailText).Name,
            ReflectExtensions.GetPath<ContactEmailInfo>(p => p.Email));

        public EmailCell(IntPtr handle) : base(Bindings, handle)
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

        public string EmailText {
            get { return EmailLabel.Text; }
            set { EmailLabel.Text = value; }
        }

        public static EmailCell Create()
        {
            return (EmailCell)Nib.Instantiate(null, null)[0];
        }
    }
}
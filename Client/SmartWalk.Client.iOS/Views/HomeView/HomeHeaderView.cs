using System;
using Cirrious.CrossCore.Core;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.ViewModels;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Views.HomeView
{
    public partial class HomeHeaderView : UICollectionReusableView, IMvxBindable
    {
        public static readonly UINib Nib = UINib.FromName("HomeHeaderView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("HomeHeaderView");

        public const float DefaultHeight = 44;

        public HomeHeaderView(IntPtr handle) : base(handle)
        {
            this.CreateBindingContext();
        }

        public IMvxBindingContext BindingContext { get; set; }

        public HomeViewModel DataContext
        {
            get { return (HomeViewModel)BindingContext.DataContext; }
            set { BindingContext.DataContext = value; }
        }

        object IMvxDataConsumer.DataContext
        {
            get { return BindingContext.DataContext; }
            set { BindingContext.DataContext = value; }
        }

        public static HomeHeaderView Create()
        {
            return (HomeHeaderView)Nib.Instantiate(null, null)[0];
        }

        public void Initialize()
        {
            this.CreateBinding(TitleLabel)
                .To<HomeViewModel>(vm => vm.Title)
                .Apply();

            InitializeStyle();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                BindingContext.ClearAllBindings();
            }

            ConsoleUtil.LogDisposed(this);
            base.Dispose(disposing);
        }

        private void InitializeStyle()
        {
            BackgroundColor = UIColor.White;

            TitleLabel.Font = Theme.HomeHeaderFont;
            TitleLabel.TextColor = Theme.CellText;
        }
    }
}
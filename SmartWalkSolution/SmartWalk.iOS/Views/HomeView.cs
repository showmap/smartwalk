using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Core.ViewModels;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.Binding.Bindings;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Binders;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace SmartWalk.iOS.Views
{
    public partial class HomeView : MvxViewController
    {
        public HomeView() : base ("HomeView", null)
        {
        }

        public new IHomeViewModel ViewModel
        {
            get { return (IHomeViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var bindingSet = this.CreateBindingSet<HomeView, IHomeViewModel>();

            bindingSet.Bind(HelloWorldsLabel).For(p => p.Text).To(p => p.TestDateLabel);

            bindingSet.Apply();
        }

        partial void OnPushMeButtonClick(UIButton sender, UIEvent @event)
        {
            ViewModel.UpdateLabel();
        }
    }
}
using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace SmartWalk.iOS.Controls
{
    [Register("UIProgressImageView")]
    public class UIProgressImageView : UIImageView
    {
        private readonly UIActivityIndicatorView _progress = 
            new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray)
                {
                    HidesWhenStopped = true
                };

        public UIProgressImageView()
        {
            Add(_progress);
        }

        public UIProgressImageView(IntPtr handle) : base(handle) 
        {
            Add(_progress);
        }

        public UIActivityIndicatorViewStyle ActivityIndicatorViewStyle
        {
            get { return _progress.ActivityIndicatorViewStyle; }
            set { _progress.ActivityIndicatorViewStyle = value; }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _progress.Frame = Bounds;
        }

        public void StartProgress()
        {
            _progress.StartAnimating();
        }

        public void StopProgress()
        {
            _progress.StopAnimating();
        }
    }
}
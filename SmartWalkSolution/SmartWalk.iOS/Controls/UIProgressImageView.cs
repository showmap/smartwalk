using System;
using MonoTouch.UIKit;

namespace SmartWalk.iOS.Controls
{
    public class UIProgressImageView : UIImageView
    {
        private readonly UIActivityIndicatorView _progress;

        public UIProgressImageView()
        {
            _progress = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray)
                {
                    HidesWhenStopped = true
                };

            Add(_progress);
        }

        public UIProgressImageView(IntPtr handle) : base(handle) {}

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
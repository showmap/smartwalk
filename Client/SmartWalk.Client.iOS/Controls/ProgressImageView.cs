using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS.Controls
{
    [Register("ProgressImageView")]
    public class ProgressImageView : UIImageView
    {
        private readonly UIActivityIndicatorView _progress = 
            new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray)
                {
                    HidesWhenStopped = true
                };

        public ProgressImageView()
        {
            Add(_progress);
        }

        public ProgressImageView(IntPtr handle) : base(handle) 
        {
            Add(_progress);
        }

        public UIActivityIndicatorViewStyle ActivityIndicatorViewStyle
        {
            get { return _progress.ActivityIndicatorViewStyle; }
            set { _progress.ActivityIndicatorViewStyle = value; }
        }

        public override UIImage Image
        {
            get { return base.Image; }
            set
            {
                base.Image = value;

                if (this.ProgressEnded())
                {
                    StopProgress();
                }
            }
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ConsoleUtil.LogDisposed(this);
        }
    }
}
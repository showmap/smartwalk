using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS.Controls
{
    [Register("Shadow")]
    public class Shadow : UIImageView
    {
        public Shadow()
        {
            Initialize();
        }

        public Shadow(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        private void Initialize()
        {
            Image = Theme.ShadowImage;
        }
    }
}
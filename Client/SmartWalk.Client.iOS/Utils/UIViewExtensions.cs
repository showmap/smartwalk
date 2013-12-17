using MonoTouch.UIKit;

namespace SmartWalk.iOS.Utils
{
    public static class UIViewExtensions
    {
        public static void RemoveSubviews(this UIView view)
        {
            foreach (var subView in view.Subviews)
            {
                subView.RemoveFromSuperview();
            }
        }
    }
}
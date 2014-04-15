using MonoTouch.UIKit;
using SmartWalk.Client.Core.Services;

namespace SmartWalk.Client.iOS.Services
{
    public class Clipboard : IClipboard
    {
        public void Copy(string str)
        {
            UIPasteboard.General.String = str;
        }

        public string Paste()
        {
            return UIPasteboard.General.String;
        }
    }
}
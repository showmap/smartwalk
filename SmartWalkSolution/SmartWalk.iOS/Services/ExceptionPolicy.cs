using System;
using MonoTouch.UIKit;
using SmartWalk.Core.Services;

namespace SmartWalk.iOS.Services
{
    public class ExceptionPolicy : IExceptionPolicy
    {
        public void Trace(Exception ex)
        {
            var alert = new UIAlertView("Exception", ex.Message, null, "OK", null);

            alert.Show();
        }
    }
}
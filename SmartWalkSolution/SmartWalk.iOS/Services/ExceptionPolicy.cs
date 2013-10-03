using System;
using MonoTouch.UIKit;
using SmartWalk.Core.Services;

namespace SmartWalk.iOS.Services
{
    public class ExceptionPolicy : IExceptionPolicy
    {
        private readonly IAnalyticsService _analyticsService;

        public ExceptionPolicy(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public void Trace(Exception ex)
        {
            _analyticsService.SendException(false, ex.ToString());

            var alert = new UIAlertView("Exception", ex.Message, null, "OK", null);
            alert.Show();
        }
    }
}
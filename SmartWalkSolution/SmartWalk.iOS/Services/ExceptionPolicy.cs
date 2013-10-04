using System;
using MonoTouch.UIKit;
using SmartWalk.Core.Services;
using System.Net;

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

            // TODO: To Localize
            string title = "Error";
            string message = ex.Message;

            if (ex is WebException)
            {
                title = "Network Error";
                message = "Can't access network content. Please try again later";
            }

            var alert = new UIAlertView(title, message, null, "OK", null);
            alert.Show();
        }
    }
}
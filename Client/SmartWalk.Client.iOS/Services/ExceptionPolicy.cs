using System;
using System.Net;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Services;

namespace SmartWalk.Client.iOS.Services
{
    public class ExceptionPolicy : IExceptionPolicy
    {
        private readonly IAnalyticsService _analyticsService;

        public ExceptionPolicy(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public void Trace(Exception ex, bool showAlert = true)
        {
            _analyticsService.SendException(false, ex.ToString());

            if (showAlert)
            {
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
}
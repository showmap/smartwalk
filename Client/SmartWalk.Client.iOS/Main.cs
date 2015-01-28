using System;
using GoogleAnalytics;
using UIKit;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Services;

namespace SmartWalk.Client.iOS
{
    public class Application
    {
        static void Main(string[] args)
        {
            try
            {
                UIApplication.Main(args, null, "AppDelegate");
            }
            catch (Exception ex)
            {
                if (!AnalyticsService.IsOptOut)
                {
                    EasyTracker.Current
                        .OnApplicationUnhandledException(ex)
                        .ContinueWithThrow();
                }

                throw;
            }
        }
    }
}
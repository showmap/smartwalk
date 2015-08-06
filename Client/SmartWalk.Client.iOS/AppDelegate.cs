using System;
using System.Threading;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.ViewModels;
using Foundation;
using GoogleAnalytics;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Services;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.Settings;
using UIKit;

namespace SmartWalk.Client.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : MvxApplicationDelegate
    {
        private string _version;
        private AppSettings _settings;

        internal static new UIWindow Window { get; private set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            UISynchronizationContext.Current = SynchronizationContext.Current;

            _settings = AppSettingsUtil.LoadSettings();

            InitializeVersion();
            InitializeUserDefaults();

#if ADHOC || APPSTORE
            InitializeGAI();
#else
            AnalyticsService.IsOptOut = true;
#endif

            AppSettingsUtil.HandleResetCache(_settings);

            Theme.Apply();

            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            var setup = new Setup(this, Window, _settings, _version);
            setup.Initialize();

            var startup = Mvx.Resolve<IMvxAppStart>();
            startup.Start();

            NavBarManager.Instance.SetNativeHidden(true, false);
            NavBarManager.Instance.SetHidden(false, false);

            Window.MakeKeyAndVisible();

            Mvx.Resolve<ICloudService>();
            
            return true;
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, 
            string sourceApplication, NSObject annotation)
        {
            var uri = url != null && !string.IsNullOrWhiteSpace(url.ToString()) 
                ? new Uri(url.ToString()) : null;
            var service = Mvx.Resolve<IDeeplinkingService>();
            return service.NavigateView(uri);
        }

        public override void DidEnterBackground(UIApplication application)
        {
            base.DidEnterBackground(application);

            if (!AnalyticsService.IsOptOut)
            {
                EasyTracker.Current.OnApplicationDeactivated(application);
            }
        }

        public override void WillEnterForeground(UIApplication application)
        {
            base.WillEnterForeground(application);

            if (!AnalyticsService.IsOptOut)
            {
                EasyTracker.Current.OnApplicationActivated(application);
            }

            Mvx.Resolve<ILocationService>().RefreshLocation();

            AppSettingsUtil.HandleResetCache(_settings);
        }

        public override void DidChangeStatusBarOrientation(
            UIApplication application, 
            UIInterfaceOrientation oldStatusBarOrientation)
        {
            NavBarManager.Instance.Layout();
        }

        private void InitializeVersion()
        {
            using (var versionString = new NSString(SettingKeys.CFBundleShortVersionString))
            {
                _version = NSBundle.MainBundle.InfoDictionary[versionString].ToString();
            }
        }

        private void InitializeUserDefaults()
        {
            NSUserDefaults.StandardUserDefaults[SettingKeys.VersionNumber] = new NSString(_version);
        }

#if ADHOC || APPSTORE
        private static void InitializeGAI()
        {
            EasyTracker.GetTracker();
            EasyTracker.Current.Config.ReportUncaughtExceptions = true;
            EasyTracker.Current.Config.AutoAppLifetimeTracking = true;

            if (NSUserDefaults.StandardUserDefaults[SettingKeys.AnonymousStatsEnabled] != null)
            {
                AnalyticsService.IsOptOut = 
                    !NSUserDefaults.StandardUserDefaults
                        .BoolForKey(SettingKeys.AnonymousStatsEnabled);
            }
        }
#endif
    }
}
using System.Drawing;
using System.Threading;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using GoogleAnalytics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Services;
using SmartWalk.Client.iOS.Utils;
#if ADHOC
using MonoTouch.TestFlight;
#endif

namespace SmartWalk.Client.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : MvxApplicationDelegate
    {
        private string _version;
        private Settings _settings;

        internal static new UIWindow Window { get; private set; }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            UISynchronizationContext.Current = SynchronizationContext.Current;

            _settings = SettingsUtil.LoadSettings();

#if ADHOC
            InitializeTestFlight();
#endif

            InitializeVersion();
            InitializeUserDefaults();

#if ADHOC || APPSTORE
            InitializeGAI();
#else
            GoogleAnalyticsService.IsOptOut = true;
#endif

            SettingsUtil.HandleResetCache(_settings);

            Theme.Apply();

            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            var presenter = new MvxTouchViewPresenter(this, Window);

            var setup = new Setup(this, presenter, _settings);
            setup.Initialize();

            var startup = Mvx.Resolve<IMvxAppStart>();
            startup.Start();

            Window.MakeKeyAndVisible();
            
            return true;
        }

        public override void DidEnterBackground(UIApplication application)
        {
            base.DidEnterBackground(application);

            if (!GoogleAnalyticsService.IsOptOut)
            {
                EasyTracker.Current.OnApplicationDeactivated(application);
            }
        }

        public override void WillEnterForeground(UIApplication application)
        {
            base.WillEnterForeground(application);

            if (!GoogleAnalyticsService.IsOptOut)
            {
                EasyTracker.Current.OnApplicationActivated(application);
            }

            SettingsUtil.HandleResetCache(_settings);
        }

        public override void WillChangeStatusBarFrame(
            UIApplication application, 
            RectangleF newStatusBarFrame)
        {
            if (Window != null)
            {
                NavBarManager.Instance.Layout();
            }
        }

        public override void DidChangeStatusBarOrientation(
            UIApplication application, 
            UIInterfaceOrientation oldStatusBarOrientation)
        {
            if (Window != null)
            {
                NavBarManager.Instance.Layout();
            }
        }

#if ADHOC
        private void InitializeTestFlight()
        {
            TestFlight.TakeOffThreadSafe(_settings.TestFlightToken);
        }
#endif

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
        private void InitializeGAI()
        {
            EasyTracker.GetTracker();
            EasyTracker.Current.Config.TrackingId = _settings.TrackingId;
            EasyTracker.Current.Config.ReportUncaughtExceptions = true;
            EasyTracker.Current.Config.AutoAppLifetimeTracking = true;

            if (NSUserDefaults.StandardUserDefaults[SettingKeys.AnonymousStatsEnabled] != null)
            {
                GoogleAnalyticsService.IsOptOut = 
                    !NSUserDefaults.StandardUserDefaults
                        .BoolForKey(SettingKeys.AnonymousStatsEnabled);
            }
        }
#endif
    }
}

using System.Threading;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using GoogleAnalytics;
using MonoTouch.Foundation;
#if ADHOC
using MonoTouch.TestFlight;
#endif
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Services;
using SmartWalk.Client.iOS.Utils;

namespace SmartWalk.Client.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : MvxApplicationDelegate
    {
        private static string _version;

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            UISynchronizationContext.Current = SynchronizationContext.Current;

#if ADHOC
            InitializeTestFlight();
#endif

            InitializeVersion();
            InitializeSettings();

#if ADHOC || APPSTORE
            InitializeGAI();
#else
            GoogleAnalyticsService.IsOptOut = true;
#endif

            Theme.Apply();

            var window = new UIWindow(UIScreen.MainScreen.Bounds);
            NavBarManager.Initialize(window);

            var presenter = new MvxTouchViewPresenter(this, window);

            var setup = new Setup(this, presenter);
            setup.Initialize();

            var startup = Mvx.Resolve<IMvxAppStart>();
            startup.Start();

            window.MakeKeyAndVisible();
            
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
        }

        public override void DidChangeStatusBarOrientation(
            UIApplication application, 
            UIInterfaceOrientation oldStatusBarOrientation)
        {
            NavBarManager.Instance.Rotate();
        }

#if ADHOC
        private static void InitializeTestFlight()
        {
            TestFlight.TakeOffThreadSafe("23af84a9-44e6-4716-996d-a4f5dd72d6ba");
        }
#endif

        private static void InitializeVersion()
        {
            using (var versionString = new NSString(SettingKeys.CFBundleShortVersionString))
            {
                _version = NSBundle.MainBundle.InfoDictionary[versionString].ToString();
            }
        }

        private static void InitializeSettings()
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
                GoogleAnalyticsService.IsOptOut = 
                    !NSUserDefaults.StandardUserDefaults
                        .BoolForKey(SettingKeys.AnonymousStatsEnabled);
            }
        }
#endif
    }
}

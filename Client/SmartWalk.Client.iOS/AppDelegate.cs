using System.Drawing;
using System.Threading;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.ViewModels;
using GoogleAnalytics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.iOS.Resources;
using SmartWalk.Client.iOS.Services;
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.Core.Services;
#if ADHOC
using MonoTouch.TestFlight;
#endif

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

#if ADHOC
            InitializeTestFlight();
#endif

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

            var setup = new Setup(this, Window, _settings);
            setup.Initialize();

            var startup = Mvx.Resolve<IMvxAppStart>();
            startup.Start();

            ((UINavigationController)AppDelegate.Window.RootViewController)
                .NavigationBarHidden = true;
            NavBarManager.Instance.SetHidden(false, false);

            Window.MakeKeyAndVisible();
            
            return true;
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, 
            string sourceApplication, NSObject annotation)
        {
            var service = Mvx.Resolve<IDeeplinkingService>();
            return service.NavigateView(url.ToString());
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

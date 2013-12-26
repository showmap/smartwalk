using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using GoogleAnalytics;
using MonoTouch.Foundation;
using MonoTouch.TestFlight;
using MonoTouch.UIKit;
using SmartWalk.Client.Core.Constants;
using SmartWalk.Client.iOS.Resources;

namespace SmartWalk.Client.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : MvxApplicationDelegate
    {
        private UIWindow _window;
        private static string _version;

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
#if ADHOC
            InitializeTestFlight();
#endif

            InitializeVersion();
            InitializeSettings();

#if APPSTORE
            InitializeGAI();
#endif

            Theme.Apply();

            _window = new UIWindow(UIScreen.MainScreen.Bounds);

            var presenter = new MvxTouchViewPresenter(this, _window);

            var setup = new Setup(this, presenter);
            setup.Initialize();

            var startup = Mvx.Resolve<IMvxAppStart>();
            startup.Start();

            _window.MakeKeyAndVisible();
            
            return true;
        }

        private static void InitializeTestFlight()
        {
            TestFlight.TakeOffThreadSafe("23af84a9-44e6-4716-996d-a4f5dd72d6ba");
        }

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

        private static void InitializeGAI()
        {
            GAI.SharedInstance.TrackUncaughtExceptions = true;
            GAI.SharedInstance.DispatchInterval = 20;
            GAI.SharedInstance.GetTracker("UA-44480601-1");
            GAI.SharedInstance.DefaultTracker.AppVersion = _version;

            if (NSUserDefaults.StandardUserDefaults[SettingKeys.AnonymousStatsEnabled] != null)
            {
                GAI.SharedInstance.OptOut = !NSUserDefaults.StandardUserDefaults
                    .BoolForKey(SettingKeys.AnonymousStatsEnabled);
            }
        }
    }
}
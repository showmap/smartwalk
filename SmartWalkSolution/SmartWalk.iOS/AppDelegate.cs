using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using GoogleAnalytics;
using MonoTouch.Foundation;
using MonoTouch.TestFlight;
using MonoTouch.UIKit;
using SmartWalk.iOS.Resources;

namespace SmartWalk.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : MvxApplicationDelegate
	{
		private UIWindow _window;

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
            // TODO: Must be commented before Release
            TestFlight.TakeOffThreadSafe("23af84a9-44e6-4716-996d-a4f5dd72d6ba");

            InitializeGAI();

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

        private static void InitializeGAI()
        {
            GAI.SharedInstance.TrackUncaughtExceptions = true;
            GAI.SharedInstance.DispatchInterval = 20;
            GAI.SharedInstance.GetTracker("UA-44480601-1");

            using (var versionString = new NSString("CFBundleVersion"))
            {
                GAI.SharedInstance.DefaultTracker.AppVersion = 
                    NSBundle.MainBundle.InfoDictionary[versionString].ToString();
            }

            // TODO: To save user setting for OptOut
#if DEBUG
            GAI.SharedInstance.OptOut = true;
#endif
        }
	}
}
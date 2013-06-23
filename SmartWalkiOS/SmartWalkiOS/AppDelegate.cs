using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace SmartWalkiOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		private UIWindow _window;
		private UINavigationController _navController;
		private HomeController _homeController;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			_window = new UIWindow (UIScreen.MainScreen.Bounds);
			_window.MakeKeyAndVisible();

			_navController = new UINavigationController();
			_homeController = new HomeController();

			_navController.PushViewController(_homeController, true);

			_window.RootViewController = _navController;
			_window.MakeKeyAndVisible();
			
			return true;
		}
	}
}


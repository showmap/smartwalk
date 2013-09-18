using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SmartWalk.iOS.Controls;
using SmartWalk.iOS.Views.HomeView;

namespace SmartWalk.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : MvxApplicationDelegate
	{
		private UIWindow _window;

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
            InitializeStyle();

			_window = new UIWindow(UIScreen.MainScreen.Bounds);

			var presenter = new MvxTouchViewPresenter(this, _window);

			var setup = new Setup(this, presenter);
			setup.Initialize();

			var startup = Mvx.Resolve<IMvxAppStart>();
			startup.Start();

			_window.MakeKeyAndVisible();
			
			return true;
		}

        public static void InitializeStyle()
        {
            UINavigationBar.Appearance.SetBackgroundImage(
                UIImage.FromFile("Images/NavBarBackground.png"), 
                UIBarMetrics.Default);
            UINavigationBar.Appearance.ShadowImage = 
                UIImage.FromFile("Images/NavBarShadow.png");

            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { 
                Font = UIFont.FromName("BrandonGrotesque-Black", 14),
                TextColor = ThemeColors.NavBarText,
                TextShadowColor = UIColor.Clear,
                TextShadowOffset = new UIOffset(0, 0)
            });
            UINavigationBar.Appearance.SetTitleVerticalPositionAdjustment(2, UIBarMetrics.Default);

            var cellAppearance = UICollectionViewCell.AppearanceWhenContainedIn(typeof(HomeView));
            cellAppearance.BackgroundColor = ThemeColors.CellBackground;

            var labelAppearance = UILabel.AppearanceWhenContainedIn(typeof(OrgCell));
            labelAppearance.Font = UIFont.FromName("BrandonGrotesque-Regular", 24);
            labelAppearance.TextColor = ThemeColors.CellText;
            labelAppearance.HighlightedTextColor = ThemeColors.CellTextHighlight; // TODO: doesn't work
        }
	}
}
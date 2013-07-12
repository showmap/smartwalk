using Cirrious.CrossCore.Plugins;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core;

namespace SmartWalk.iOS
{
	public class Setup : MvxTouchSetup
	{
		public Setup (MvxApplicationDelegate appDelegate, IMvxTouchViewPresenter presenter)
			: base(appDelegate, presenter)
		{
		}

		protected override IMvxApplication CreateApp()
		{
			return new SmartWalkApplication();
		}

        protected override void AddPluginsLoaders(MvxLoaderPluginRegistry registry)
        {
            registry.AddConventionalPlugin<Cirrious.MvvmCross.Plugins.DownloadCache.Touch.Plugin>();
            registry.AddConventionalPlugin<Cirrious.MvvmCross.Plugins.File.Touch.Plugin>();

            base.AddPluginsLoaders(registry);
        }

        protected override void InitializeLastChance ()
        {
            Cirrious.MvvmCross.Plugins.DownloadCache.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.File.PluginLoader.Instance.EnsureLoaded();

            base.InitializeLastChance();
        }
	}
}
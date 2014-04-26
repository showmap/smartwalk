using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;
using Cirrious.MvvmCross.Plugins.DownloadCache;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.iOS.Services;

namespace SmartWalk.Client.iOS
{
    public class Setup : MvxTouchSetup
    {
        private const string Host = "smartwalk.azurewebsites.net";

        public Setup(MvxApplicationDelegate appDelegate, IMvxTouchViewPresenter presenter)
            : base(appDelegate, presenter)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            InitializeConfiguration();

            Mvx.LazyConstructAndRegisterSingleton<IClipboard, Clipboard>();
            Mvx.LazyConstructAndRegisterSingleton<IHttpService, HttpService>();
            Mvx.LazyConstructAndRegisterSingleton<IAnalyticsService, GoogleAnalyticsService>();
            Mvx.LazyConstructAndRegisterSingleton<IExceptionPolicy, ExceptionPolicy>();
            Mvx.LazyConstructAndRegisterSingleton<ILocationService, LocationService>();
            Mvx.LazyConstructAndRegisterSingleton<ICalendarService, CalendarService>();
            Mvx.LazyConstructAndRegisterSingleton<IReachabilityService, ReachabilityService>();
            Mvx.LazyConstructAndRegisterSingleton<ICacheService, CacheService>();
            Mvx.LazyConstructAndRegisterSingleton<IShowDirectionsTask, ShowDirectionsTask>();

            return new SmartWalkApplication();
        }

        protected override void AddPluginsLoaders(MvxLoaderPluginRegistry loaders)
        {
            loaders.AddConventionalPlugin<Cirrious.MvvmCross.Plugins.DownloadCache.Touch.Plugin>();
            loaders.AddConventionalPlugin<Cirrious.MvvmCross.Plugins.File.Touch.Plugin>();
            loaders.AddConventionalPlugin<Cirrious.MvvmCross.Plugins.PhoneCall.Touch.Plugin>();
            loaders.AddConventionalPlugin<Cirrious.MvvmCross.Plugins.Email.Touch.Plugin>();

            base.AddPluginsLoaders(loaders);
        }

        protected override void InitializeLastChance()
        {
            PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.File.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.PhoneCall.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.Email.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.Json.PluginLoader.Instance.EnsureLoaded();

            Mvx.RegisterSingleton<IMvxHttpFileDownloader>(() => new MvxFastHttpFileDownloader());

            base.InitializeLastChance();
        }

        private static void InitializeConfiguration()
        {
            Mvx.RegisterSingleton<IConfiguration>(new Configuration(Host));
        }
    }
}
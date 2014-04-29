using System;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using MonoTouch.UIKit;
using SmartWalk.Client.Core;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.iOS.Services;
using SmartWalk.Client.iOS.Utils.Mvx;

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

        protected override IMvxPluginConfiguration GetPluginConfiguration(Type plugin)
        {
            if (plugin == typeof(Cirrious.MvvmCross.Plugins.DownloadCache.PluginLoader))
            {
                return MvxPlus.GetDownloadCacheConfig();
            }

            return null;
        }

        protected override void InitializeLastChance()
        {
            Cirrious.MvvmCross.Plugins.DownloadCache.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.File.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.PhoneCall.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.Email.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.Json.PluginLoader.Instance.EnsureLoaded();

            Mvx.RegisterSingleton<Cirrious.MvvmCross.Plugins.DownloadCache.IMvxFileDownloadCache>(
                MvxPlus.CreateDownloadCache);
            Mvx.RegisterSingleton<Cirrious.MvvmCross.Plugins.DownloadCache.IMvxImageCache<UIImage>>(
                MvxPlus.CreateImageCache);
            Mvx.RegisterSingleton<Cirrious.MvvmCross.Plugins.DownloadCache.IMvxHttpFileDownloader>(
                MvxPlus.CreateHttpFileDownloader);

            base.InitializeLastChance();
        }

        private static void InitializeConfiguration()
        {
            Mvx.RegisterSingleton<IConfiguration>(new Configuration(Host));
        }
    }
}
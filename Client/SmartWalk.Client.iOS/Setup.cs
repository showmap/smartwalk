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
using SmartWalk.Client.iOS.Utils;
using SmartWalk.Client.iOS.Utils.MvvmCross;
using MvxDownloadCacheConfiguration = Cirrious.MvvmCross.Plugins.DownloadCache.Touch.MvxDownloadCacheConfiguration;

namespace SmartWalk.Client.iOS
{
    public class Setup : MvxTouchSetup
    {
        private readonly Settings _settings;

        private MvxDownloadCacheConfiguration _picsCacheConfig;
        private MvxDownloadCacheConfiguration _resizedPicsCacheConfig;
        private MvxDownloadCacheConfiguration _dataCacheConfig;

        public Setup(
            MvxApplicationDelegate appDelegate, 
            IMvxTouchViewPresenter presenter,
            Settings settings)
            : base(appDelegate, presenter)
        {
            _settings = settings;
            InitializeCacheSettings();
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
            Mvx.RegisterSingleton<ICacheService>(new CacheService(_dataCacheConfig.CacheFolderPath));
            Mvx.LazyConstructAndRegisterSingleton<IShowDirectionsTask, ShowDirectionsTask>();
            Mvx.LazyConstructAndRegisterSingleton<IOpenURLTask, OpenURLTask>();

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
            return plugin == typeof(Cirrious.MvvmCross.Plugins.DownloadCache.PluginLoader) 
                ? _picsCacheConfig
                : null;
        }

        protected override void InitializeLastChance()
        {
            Cirrious.MvvmCross.Plugins.DownloadCache.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.File.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.PhoneCall.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.Email.PluginLoader.Instance.EnsureLoaded();
            Cirrious.MvvmCross.Plugins.Json.PluginLoader.Instance.EnsureLoaded();

            Mvx.RegisterSingleton<Cirrious.MvvmCross.Plugins.DownloadCache.IMvxImageCache<UIImage>>(
                () => MvxPlus.CreateImageCache(_picsCacheConfig));

            Mvx.RegisterSingleton<IMvxResizedImageCache<UIImage>>(
                () => MvxPlus.CreateResizedImageCache(_resizedPicsCacheConfig));

            Mvx.RegisterType<IMvxResizedImageHelper<UIImage>, MvxResizedDynamicImageHelper<UIImage>>();

            Mvx.RegisterSingleton<Cirrious.MvvmCross.Plugins.DownloadCache.IMvxHttpFileDownloader>(
                MvxPlus.CreateHttpFileDownloader);

            base.InitializeLastChance();
        }

        private void InitializeConfiguration()
        {
            Mvx.RegisterSingleton<IConfiguration>(new Configuration(_settings.ServerHost));
        }

        private void InitializeCacheSettings()
        {
            foreach (var cache in _settings.Caches)
            {
                // TODO: Figure out how to deserialize this from xml
                cache.MaxFileAge = TimeSpan.FromDays(10);

                if (cache.CacheName == "Pictures.MvvmCross")
                {
                    _picsCacheConfig = cache;
                }

                if (cache.CacheName == "ResizedPictures.MvvmCross")
                {
                    _resizedPicsCacheConfig = cache;
                }

                if (cache.CacheName == "Data.SmartWalk")
                {
                    _dataCacheConfig = cache;
                }
            }
        }
    }
}
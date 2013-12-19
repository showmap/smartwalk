using Cirrious.CrossCore;
using Cirrious.MvvmCross.Platform;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.Services;
using SmartWalk.Client.Core.ViewModels;

namespace SmartWalk.Client.Core
{
    public class SmartWalkApplication : MvxApplication
    {
        public SmartWalkApplication()
        {
            Mvx.LazyConstructAndRegisterSingleton<ICacheService, CacheService>();
            Mvx.LazyConstructAndRegisterSingleton<ILocationService, LocationService>();
            Mvx.LazyConstructAndRegisterSingleton<ISmartWalkDataService, SmartWalkDataService>();

            Mvx.RegisterSingleton<IMvxAppStart>(new MvxAppStart<HomeViewModel>());

            var parserDir = Mvx.Resolve<IMvxFillableStringToTypeParser>();
            parserDir.ExtraParsers.Add(new AddressesParser());
        }
    }
}
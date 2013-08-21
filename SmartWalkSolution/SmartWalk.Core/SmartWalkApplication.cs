using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Services;
using SmartWalk.Core.ViewModels;

namespace SmartWalk.Core
{
	public class SmartWalkApplication : MvxApplication
	{
		public SmartWalkApplication()
		{
            Mvx.ConstructAndRegisterSingleton<ICacheService, CacheService>();
            Mvx.ConstructAndRegisterSingleton<ISmartWalkDataService, SmartWalkDataService>();
            Mvx.RegisterSingleton<IMvxAppStart>(new MvxAppStart<HomeViewModel>());
		}
	}
}
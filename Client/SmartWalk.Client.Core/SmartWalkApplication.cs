using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using Cirrious.MvvmCross.Platform;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Client.Core.ViewModels;

namespace SmartWalk.Client.Core
{
    public class SmartWalkApplication : MvxApplication
    {
        public SmartWalkApplication()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            RegisterAppStart<HomeViewModel>();

            var parserDir = Mvx.Resolve<IMvxFillableStringToTypeParser>();
            parserDir.ExtraParsers.Add(new AddressesParser());
        }
    }
}
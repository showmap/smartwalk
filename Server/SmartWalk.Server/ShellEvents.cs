using System.Web.Mvc;
using Orchard.Environment;
using SmartWalk.Server.Providers;
using SmartWalk.Server.Utils;
using SmartWalk.Shared;

namespace SmartWalk.Server
{
    [UsedImplicitly]
    public class ShellEvents : IOrchardShellEvents
    {
        public void Activated()
        {
            JsonDotNetValueProviderFactory.RegisterFactory();
            ModelBinders.Binders.DefaultBinder = new EnumConverterModelBinder();
            FileUtil.CleanupUploadedImageStorage();
        }

        public void Terminating()
        {
            FileUtil.CleanupUploadedImageStorage();
        }
    }
}
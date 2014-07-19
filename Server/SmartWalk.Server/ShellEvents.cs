using Orchard.Environment;
using SmartWalk.Server.Providers;
using SmartWalk.Shared;

namespace SmartWalk.Server
{
    [UsedImplicitly]
    public class ShellEvents : IOrchardShellEvents
    {
        public void Activated()
        {
            JsonDotNetValueProviderFactory.RegisterFactory();
        }

        public void Terminating()
        {
        }
    }
}
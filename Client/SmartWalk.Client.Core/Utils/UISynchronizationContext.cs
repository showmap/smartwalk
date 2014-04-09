using System.Threading;

namespace SmartWalk.Client.Core.Utils
{
    public static class UISynchronizationContext
    {
        public static SynchronizationContext Current { get; set; }
    }
}
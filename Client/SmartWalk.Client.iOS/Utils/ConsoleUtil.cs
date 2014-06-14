using System;

namespace SmartWalk.Client.iOS.Utils
{
    public static class ConsoleUtil
    {
        public static void LogDisposed(object obj)
        {
#if DEBUG
            Console.WriteLine("Disposed: {0} ({1})", obj.GetType().Name, obj.GetHashCode());
#endif
        }
    }
}
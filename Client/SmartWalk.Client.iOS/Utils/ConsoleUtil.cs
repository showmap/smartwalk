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

        public static void Trace(string str, params object[] args)
        {
#if DEBUG
            Console.WriteLine(str, args);
#endif
        }
    }
}
using System;

namespace SmartWalk.Core.Utils
{
    public static class ConsoleUtil
    {
        public static void Log(string line, params object[] arg)
        {
#if DEBUG
            Console.WriteLine(line, arg);
#endif
        }

        public static void LogDisposed(object obj)
        {
#if DEBUG
            Console.WriteLine("Disposed: {0} ({1})", obj.GetType().Name, obj.GetHashCode());
#endif
        }
    }
}
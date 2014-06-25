using System.Reflection;

namespace SmartWalk.Server.Common.Utils
{
    public static class VersionUtil
    {
        private static string _version;

        public static string CurrentVersion
        {
            get
            {
                if (_version == null)
                {
                    var version = 
                        typeof(VersionUtil).GetTypeInfo().Assembly.GetName().Version;
                    _version = version.ToString();
                }

                return _version;
            }
        }
    }
}
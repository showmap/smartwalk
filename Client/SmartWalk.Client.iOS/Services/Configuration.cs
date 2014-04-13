using SmartWalk.Client.Core.Services;

namespace SmartWalk.Client.iOS.Services
{
    public class Configuration : IConfiguration
    {
        public string Host { get; set; }

        public string Api { get; set; }
    }
}
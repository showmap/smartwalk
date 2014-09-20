using System;
using System.Xml.Serialization;
using SmartWalk.Client.iOS.Utils.MvvmCross;

namespace SmartWalk.Client.iOS.Utils
{
    [Serializable]
    [XmlRoot(ElementName = "settings")]
    public class AppSettings
    {
        private int _postponeTimeMinutes;

        [XmlElement("testFlightToken")]
        public string TestFlightToken { get;set; }

        [XmlElement("serverHost")]
        public string ServerHost { get;set; }

        [XmlElement("debugServerHost")]
        public string DebugServerHost { get;set; }

        [XmlElement("postponeTimeMinutes")]
        public int PostponeTimeMinutes
        {
            get
            {
                return _postponeTimeMinutes;
            }
            set
            {
                _postponeTimeMinutes = value;
                PostponeTime = TimeSpan.FromMinutes(_postponeTimeMinutes);
            }
        }

        public TimeSpan PostponeTime { get;set; }

        [XmlElement("cachesPath")]
        public string CachesPath { get;set; }

        [XmlArray("caches")]
        [XmlArrayItem("cache")]
        public CacheConfiguration[] Caches { get;set; }
    }
}
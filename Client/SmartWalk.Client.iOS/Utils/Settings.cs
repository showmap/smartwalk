using System;
using System.Xml.Serialization;
using Cirrious.MvvmCross.Plugins.DownloadCache.Touch;

namespace SmartWalk.Client.iOS.Utils
{
    [Serializable]
    [XmlRoot(ElementName = "settings")]
    public class Settings
    {
        [XmlElement("trackingId")]
        public string TrackingId { get;set; }

        [XmlElement("testFlightToken")]
        public string TestFlightToken { get;set; }

        [XmlElement("serverHost")]
        public string ServerHost { get;set; }

        [XmlArray("caches")]
        [XmlArrayItem("cache")]
        public MvxDownloadCacheConfiguration[] Caches { get;set; }
    }
}
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace SmartWalk.XmlData.Model
{
    [Serializable]
    public class EventRef
    {
        [XmlAttribute("date")]
        public string Date { get; set; }

        [XmlAttribute("hasSchedule")]
        public bool HasSchedule { get; set; }

        public DateTime DateObject
        {
            get { return DateTime.Parse(Date, CultureInfo.InvariantCulture); }
        }
    }
}

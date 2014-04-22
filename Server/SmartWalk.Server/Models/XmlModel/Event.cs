using System;
using System.Xml.Serialization;

namespace SmartWalk.Server.Models.XmlModel
{
    [XmlRoot(ElementName = "event")]
    public class Event
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlAttribute("logo")]
        public string Logo { get; set; }

        [XmlAttribute("organization")]
        public string Organization { get; set; }

        [XmlAttribute("start")]
        public string StartDate { get; set; }

        [XmlIgnore]
        public DateTime StartDateObject { get; set; }

        [XmlAttribute("modifiedBy")]
        public string ModifiedBy { get; set; }

        [XmlArray("program")]
        [XmlArrayItem("venue")]
        public Venue[] Venues { get; set; }
    }
}

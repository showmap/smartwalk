using System.Xml.Serialization;

namespace SmartWalk.Server.Models.XmlModel
{
    [XmlRoot(ElementName = "event")]
    public class Event
    {
        [XmlAttribute("organization")]
        public string Organization { get; set; }

        [XmlAttribute("start")]
        public string Start { get; set; }

        [XmlAttribute("modifiedBy")]
        public string ModifiedBy { get; set; }

        [XmlArray("program")]
        [XmlArrayItem("venue")]
        public Venue[] Venues { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace SmartWalk.Server.Models.XmlModel
{
    [Serializable]
    [XmlRoot(ElementName = "organization")]
    public class Organization
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("logo")]
        public string Logo { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlArray("contact")]
        [XmlArrayItem("phone", typeof(Phone))]
        [XmlArrayItem("email", typeof(Email))]
        [XmlArrayItem("web", typeof(Web))]
        public Contact[] Contacts { get; set; }

        [XmlArray("events")]
        [XmlArrayItem("event")]
        public EventRef[] EventRefs { get; set; }

        [XmlIgnore]
        public Event[] Events { get; set; }
    }
}

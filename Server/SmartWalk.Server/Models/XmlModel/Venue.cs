using System;
using System.Xml.Serialization;

namespace SmartWalk.Server.Models.XmlModel
{
    [Serializable]
    public class Venue
    {
        [XmlAttribute("number")]
        public string Number { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("logo")]
        public string Logo { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlArray("address")]
        [XmlArrayItem("point")]
        public Address[] Addresses { get; set; }

        [XmlArray("contact")]
        [XmlArrayItem("phone", typeof(Phone))]
        [XmlArrayItem("email", typeof(Email))]
        [XmlArrayItem("web", typeof(Web))]
        public Contact[] Contacts { get; set; }

        [XmlArray("shows")]
        [XmlArrayItem("show")]
        public Show[] Shows { get; set; }
    }
}

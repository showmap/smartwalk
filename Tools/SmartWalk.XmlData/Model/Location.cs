using System.Xml.Serialization;

namespace SmartWalk.XmlData.Model
{
    [XmlRoot(ElementName = "location")]
    public class Location
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlArray("organizations")]
        [XmlArrayItem("organization")]
        public Organization[] Organizations { get; set; }
    }
}

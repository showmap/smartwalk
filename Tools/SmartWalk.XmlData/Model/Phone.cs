using System;
using System.Xml.Serialization;

namespace SmartWalk.XmlData.Model
{
    [Serializable]
    public class Phone : Contact
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}

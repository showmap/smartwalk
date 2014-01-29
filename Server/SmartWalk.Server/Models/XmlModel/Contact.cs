using System;
using System.Xml.Serialization;

namespace SmartWalk.Server.Models.XmlModel
{
    [Serializable]
    public class Contact
    {
        [XmlText]
        public string Value { get; set; }
    }
}

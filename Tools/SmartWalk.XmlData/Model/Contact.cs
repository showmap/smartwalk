using System;
using System.Xml.Serialization;

namespace SmartWalk.XmlData.Model
{
    [Serializable]
    public class Contact
    {
        [XmlText]
        public string Value { get; set; }
    }
}

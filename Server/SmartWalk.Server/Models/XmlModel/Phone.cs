﻿using System;
using System.Xml.Serialization;

namespace SmartWalk.Server.Models.XmlModel
{
    [Serializable]
    public class Phone : Contact
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}

using System;
using System.Globalization;
using System.Xml.Serialization;

namespace SmartWalk.Server.Models.XmlModel
{
    [Serializable]
    public class EventRef
    {
        private string _date;

        [XmlAttribute("date")]
        public string Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;

                    var parsedStartDate = default(DateTime);
                    if (_date != null)
                    {
                        DateTime.TryParse(
                            _date,
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out parsedStartDate);
                    }

                    DateObject = parsedStartDate;
                }
            }
        }

        [XmlAttribute("hasSchedule")]
        public bool HasSchedule { get; set; }

        [XmlIgnore]
        public DateTime DateObject { get; set; }
    }
}

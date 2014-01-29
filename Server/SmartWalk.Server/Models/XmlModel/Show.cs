using System;
using System.Globalization;
using System.Xml.Serialization;

namespace SmartWalk.Server.Models.XmlModel
{
    [Serializable]
    public class Show
    {
        [XmlText]
        public string Desciption { get; set; }

        [XmlAttribute("start")]
        public string StartTime { get; set; }

        [XmlAttribute("end")]
        public string EndTime { get; set; }

        [XmlAttribute("logo")]
        public string Logo { get; set; }

        [XmlAttribute("web")]
        public string Web { get; set; }

        [XmlIgnore]
        public DateTime StartTimeObject { get; set; }

        [XmlIgnore]
        public DateTime EndTimeObject { get; set; }

        public void ParseShowTime(
            DateTime eventDate)
        {
            DateTime parsedStartTime;
            if (StartTime != null && DateTime.TryParse(
                    StartTime,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out parsedStartTime))
            {
                parsedStartTime = new DateTime(
                    eventDate.Year,
                    eventDate.Month,
                    eventDate.Day,
                    parsedStartTime.Hour,
                    parsedStartTime.Minute,
                    0);
            }
            else
            {
                parsedStartTime = DateTime.MinValue;
            }

            DateTime parsedEndTime;
            if (EndTime != null && DateTime.TryParse(
                    EndTime,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out parsedEndTime))
            {
                parsedEndTime = new DateTime(
                    eventDate.Year,
                    eventDate.Month,
                    eventDate.Day,
                    parsedEndTime.Hour,
                    parsedEndTime.Minute,
                    0);

                // all night AM time should be set to the next day
                if (parsedStartTime > parsedEndTime)
                {
                    parsedEndTime = parsedEndTime.AddDays(1);
                }
            }
            else
            {
                parsedEndTime = DateTime.MaxValue;
            }

            StartTimeObject = parsedStartTime;
            EndTimeObject = parsedEndTime;
        }
    }
}

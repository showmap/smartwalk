using System;
using System.Globalization;
using System.Xml.Serialization;

namespace SmartWalk.Server.Models.XmlModel
{
    [Serializable]
    public class Address
    {
        private string _coordinates;

        [XmlText]
        public string Text { get; set; }

        [XmlAttribute("coordinates")]
        public string Coordinates
        {
            get { return _coordinates; }
            set
            {
                _coordinates = value;
                UpdateLatLong();
            }
        }

        [XmlAttribute("tip")]
        public string Tip { get; set; }

        [XmlIgnore]
        public double Latitude { get; set; }

        [XmlIgnore]
        public double Longitude { get; set; }

        private void UpdateLatLong()
        {
            var latitude = default(double);
            var longitude = default(double);

            if (Coordinates != null)
            {
                var points = Coordinates.Split(',');

                if (points.Length == 2)
                {
                    double.TryParse(
                        points[0],
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out latitude);
                    double.TryParse(
                        points[1],
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out longitude);
                }
            }

            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
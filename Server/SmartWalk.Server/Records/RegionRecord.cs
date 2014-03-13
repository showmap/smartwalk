using System.Collections.Generic;
namespace SmartWalk.Server.Records
{
    public class RegionRecord
    {
        public virtual int Id { get; set; }

        public virtual string Region { get; set; }
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
    }
}
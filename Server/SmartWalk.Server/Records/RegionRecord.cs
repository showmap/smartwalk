using System.Collections.Generic;
namespace SmartWalk.Server.Records
{
    public class RegionRecord
    {
        private IList<AddressRecord> _addresses;

        public virtual int Id { get; set; }

        public virtual string Region { get; set; }
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }

        public virtual IList<AddressRecord> Addresses
        {
            get { return _addresses; }
            set { _addresses = value; }
        }

        public RegionRecord() {
            _addresses = new List<AddressRecord>();
        }
    }
}
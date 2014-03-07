using System.Collections.Generic;
namespace SmartWalk.Server.Records
{
    public class RegionRecord
    {
        private IList<AddressRecord> _addresses;

        public virtual int Id { get; set; }

        public virtual string Country { get; set; }
        public virtual string State { get; set; }
        public virtual string City { get; set; }


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
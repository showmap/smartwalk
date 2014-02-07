namespace SmartWalk.Server.Records
{
    public class AddressRecord
    {
        public virtual int Id { get; set; }
        public virtual EntityRecord EntityRecord { get; set; }
        public virtual RegionRecord RegionRecord { get; set; }
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        public virtual string Address { get; set; }
    }
}
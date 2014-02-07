namespace SmartWalk.Server.Records
{
    public class RegionRecord
    {
        public virtual int Id { get; set; }
        public virtual string Country { get; set; }
        public virtual string State { get; set; }
        public virtual string City { get; set; }
    }
}
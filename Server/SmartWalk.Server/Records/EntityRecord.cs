namespace SmartWalk.Server.Records
{
    public class EntityRecord
    {
        public virtual int Id { get; set; }
        public virtual SmartWalkUserRecord SmartWalkUserRecord { get; set; }
        public virtual int Type { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string Picture { get; set; }
    }

    public enum EntityType {
        Host = 0,
        Venue = 1
    }
}
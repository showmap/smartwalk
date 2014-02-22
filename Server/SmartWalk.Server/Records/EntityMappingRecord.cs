﻿namespace SmartWalk.Server.Records
{
    public class EntityMappingRecord
    {
        public virtual int Id { get; set; }
        public virtual EntityRecord EntityRecord { get; set; }
        public virtual StorageRecord StorageRecord { get; set; }
        public virtual int ExternalEntityId { get; set; }
        public virtual int Type { get; set; }
    }

    public enum EntityMappingType
    {
        None = 0,
        Page = 1,
        Place = 2,
        User = 3,
        Group = 4
    }
}
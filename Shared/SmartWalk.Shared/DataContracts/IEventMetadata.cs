namespace SmartWalk.Shared.DataContracts
{
    using System;

    public interface IEventMetadata
    {
        int Id { get; set; }
        IRegion Region { get; set; }
        IReference[] Host { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        DateTime? StartTime { get; set; }
        DateTime? EndTime { get; set; }
        CombineType? CombineType { get; set; }
        IReference[] Shows { get; set; } 
    }
}
using System;

namespace SmartWalk.Shared.DataContracts
{
    public interface IShow
    {
        int Id { get; set; }

        IReference[] Venue { get; set; }

        string Title { get; set; }

        string Description { get; set; }

        DateTime StartTime { get; set; }

        DateTime EndTime { get; set; }
        
        string Picture { get; set; }

        string DetailsUrl { get; set; }
    }
}
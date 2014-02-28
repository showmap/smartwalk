using System;

namespace SmartWalk.Shared.DataContracts
{
    public interface IShow
    {
        int Id { get; set; }
        IReference[] Venue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is an empty reference to a venue.
        /// </summary>
        /// <value><c>true</c> if this instance is reference; otherwise, <c>false</c>.</value>
        bool? IsReference { get; set; }

        string Title { get; set; }
        string Description { get; set; }
        DateTime? StartTime { get; set; }
        DateTime? EndTime { get; set; }
        string Picture { get; set; }
        string DetailsUrl { get; set; }
    }
}
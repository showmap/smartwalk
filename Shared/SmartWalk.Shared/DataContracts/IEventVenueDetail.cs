namespace SmartWalk.Shared.DataContracts
{
    public interface IEventVenueDetail
    {
        IReference[] Event { get; set; }
        IReference[] Venue { get; set; }
        int? SortOrder { get; set; }
        string Description { get; set; }
    }
}

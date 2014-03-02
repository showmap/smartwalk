namespace SmartWalk.Shared.DataContracts
{
    public interface IRegion
    {
        string Country { get; set; }
        string State { get; set; }
        string City { get; set; }
    }
}
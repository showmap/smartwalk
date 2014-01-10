namespace SmartWalk.Shared.DataContracts
{
    public interface IAddress
    {
        string Address { get; set; }

        double Latitude { get; set; }

        double Longitude { get; set; }
    }
}
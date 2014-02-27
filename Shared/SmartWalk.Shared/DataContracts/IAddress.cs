namespace SmartWalk.Shared.DataContracts
{
    public interface IAddress
    {
        string AddressText { get; set; }

        double Latitude { get; set; }

        double Longitude { get; set; }
    }
}
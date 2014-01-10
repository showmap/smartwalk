namespace SmartWalk.Shared.DataContracts
{
    public interface IReference
    {
        int Id { get; set; }

        Storages Storage { get; set; }
    }
}
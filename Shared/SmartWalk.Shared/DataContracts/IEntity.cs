namespace SmartWalk.Shared.DataContracts
{
    public interface IEntity
    {
        int Id { get; set; }

        EntityType? Type { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string Picture { get; set; }

        IContact[] Contacts { get; set; }

        IAddress[] Addresses { get; set; }
    }
}
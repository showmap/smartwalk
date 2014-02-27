using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Server.Models.DataContracts
{
    public class Entity : IEntity
    {
        public int Id { get; set; }

        public EntityType? Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Picture { get; set; }

        public IContact[] Contacts { get; set; }

        public IAddress[] Addresses { get; set; }
    }
}
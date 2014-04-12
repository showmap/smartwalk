using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.Core.Model.DataContracts
{
    public class Entity : IEntity
    {
        public int Id { get; set; }
        public EntityType? Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public Contact[] Contacts { get; set; }
        public Address[] Addresses { get; set; }

        IContact[] IEntity.Contacts { 
            get { return Contacts; } 
            set { Contacts = (Contact[])value; }
        }

        IAddress[] IEntity.Addresses { 
            get { return Addresses; } 
            set { Addresses = (Address[])value; }
        }

        public override bool Equals(object obj)
        {
            var entity = obj as Entity;
            if (entity != null)
            {
                return Id == entity.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                    .CombineHashCode(Id)
                    .CombineHashCode(Type)
                    .CombineHashCodeOrDefault(Name)
                    .CombineHashCodeOrDefault(Description)
                    .CombineHashCodeOrDefault(Picture)
                    .CombineHashCodeOrDefault(Contacts)
                    .CombineHashCodeOrDefault(Addresses);
        }
    }
}
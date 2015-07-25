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
        public Pictures Pictures { get; set; }
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
            var en = obj as Entity;
            if (en != null)
            {
                return Id == en.Id &&
                    Type == en.Type &&
                    Name == en.Name &&
                    Description == en.Description &&
                    Picture == en.Picture &&
                    Pictures == en.Pictures &&
                    Contacts.EnumerableEquals(en.Contacts) &&
                    Addresses.EnumerableEquals(en.Addresses);
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
                    .CombineHashCodeOrDefault(Pictures)
                    .CombineHashCodeOrDefault(Contacts)
                    .CombineHashCodeOrDefault(Addresses);
        }
    }
}
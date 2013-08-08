using System.Linq;
using SmartWalk.Core.Utils;

namespace SmartWalk.Core.Model
{
    public class EntityInfo : ISearchable
	{
        public string Id { get; set; }

		public string Name { get; set; }

        public string Logo { get; set; }

        public ContactInfo Contact { get; set; }

        public AddressInfo[] Addresses { get; set; }

        public string SearchableText
        {
            get
            {
                return (Name != null ? " " + Name : string.Empty) + 
                    (Contact != null ? " " + Contact.SearchableText : string.Empty) +
                        (Addresses != null 
                            ? Addresses.Select(a => a.SearchableText).Aggregate((a, b) => a + " " + b) 
                            : string.Empty);
            }
        }

        public override bool Equals(object obj)
        {
            var info = obj as EntityInfo;
            if (info != null)
            {
                return Id == info.Id &&
                    Name == info.Name &&
                        Logo == info.Logo &&
                            Equals(Contact, info.Contact) &&
                               Addresses.EnumerableEquals(info.Addresses);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCodeOrDefault(Id)
                    .CombineHashCodeOrDefault(Name)
                    .CombineHashCodeOrDefault(Logo)
                    .CombineHashCodeOrDefault(Contact)
                    .CombineHashCodeOrDefault(Addresses);
        }

        public EntityInfo Clone()
        {
            return new EntityInfo {
                Id = Id,
                Name = Name,
                Logo = Logo,
                Contact = Contact,
                Addresses = Addresses
            };
        }
	}
}
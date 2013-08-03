using SmartWalk.Core.Utils;

namespace SmartWalk.Core.Model
{
    public class EntityInfo
	{
        public string Id { get; set; }

		public string Name { get; set; }

        public string Logo { get; set; }

        public ContactInfo Contact { get; set; }

        public AddressInfo[] Addresses { get; set; }

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
	}
}
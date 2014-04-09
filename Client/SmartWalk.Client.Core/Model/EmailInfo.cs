using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Model.Interfaces;

namespace SmartWalk.Client.Core.Model
{
    public class EmailInfo : ISearchable, IContact
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Title
        {
            get
            {
                return Name;
            }
        }

        public string Contact
        {
            get
            {
                return Email;
            }
        }
        
        public ContactType Type
        {
            get
            {
                return ContactType.Email;
            } 
        }

        public string SearchableText
        {
            get
            {
                return (Name != null ? " " + Name : string.Empty) +
                    (Email != null ? " " + Email : string.Empty);
            }
        }

        public override bool Equals(object obj)
        {
            var info = obj as EmailInfo;
            if (info != null)
            {
                return Name == info.Name &&
                    Email == info.Email;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCodeOrDefault(Name)
                    .CombineHashCodeOrDefault(Email);
        }
    }
}
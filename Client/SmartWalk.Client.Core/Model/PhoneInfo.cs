using SmartWalk.Client.Core.Utils;
using SmartWalk.Client.Core.Model.Interfaces;

namespace SmartWalk.Client.Core.Model
{
    public class PhoneInfo : ISearchable, IContact
    {
        public string Name { get; set; }

        public string Phone { get; set; }

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
                return Phone;
            }
        }

        public ContactType Type
        {
            get
            {
                return ContactType.Phone;
            } 
        }

        public string SearchableText
        {
            get
            {
                return Name ?? string.Empty + " " + Phone ?? string.Empty;
            }
        }

        public override bool Equals(object obj)
        {
            var info = obj as PhoneInfo;
            if (info != null)
            {
                return Name == info.Name &&
                    Phone == info.Phone;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCodeOrDefault(Name)
                    .CombineHashCodeOrDefault(Phone);
        }
    }
}


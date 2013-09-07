using SmartWalk.Core.Utils;
using SmartWalk.Core.Model.Interfaces;

namespace SmartWalk.Core.Model
{
    public class PhoneInfo : ISearchable
    {
        public string Name { get; set; }

        public string Phone { get; set; }

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


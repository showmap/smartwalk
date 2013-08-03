using SmartWalk.Core.Utils;

namespace SmartWalk.Core.Model
{
    public class EmailInfo
    {
        public string Name { get; set; }

        public string Email { get; set; }

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
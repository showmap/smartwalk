using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Client.Core.Model.DataContracts
{
    public class Contact : IContact
    {
        public ContactType? Type { get; set; }
        public string Title { get; set; }
        public string ContactText { get; set; }

        public override bool Equals(object obj)
        {
            var cn = obj as Contact;
            if (cn != null)
            {
                return Type == cn.Type &&
                    Title == cn.Title &&
                    ContactText == cn.ContactText;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCode(Type)
                .CombineHashCodeOrDefault(Title)
                .CombineHashCodeOrDefault(ContactText);
        }
    }
}
using SmartWalk.Shared.DataContracts;

namespace SmartWalk.Client.Core.Model.DataContracts
{
    public class Contact : IContact
    {
        public ContactType? Type { get; set; }
        public string Title { get; set; }
        public string ContactText { get; set; }
    }
}
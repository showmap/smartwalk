using Newtonsoft.Json;
using SmartWalk.Shared;

namespace SmartWalk.Server.ViewModels
{
    public class ContactVm
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Contact { get; set; }

        [JsonIgnore]
        [UsedImplicitly]
        public bool Destroy { get; set; }
    }
}
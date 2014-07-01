namespace SmartWalk.Server.ViewModels
{
    public class ContactVm
    {
        public int Id { get; set; }
        public VmItemState State { get; set; }
        public int EntityId { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Contact { get; set; }
    }
}
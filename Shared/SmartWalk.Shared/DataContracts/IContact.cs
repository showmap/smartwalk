namespace SmartWalk.Shared.DataContracts
{
    public interface IContact
    {
        ContactType? Type { get; set; }
        string Title { get; set; }
        string ContactText { get; set; }
    }
}
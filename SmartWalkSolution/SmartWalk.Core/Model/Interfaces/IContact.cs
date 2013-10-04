namespace SmartWalk.Core.Model.Interfaces
{
    public interface IContact
    {
        string Title { get; }
        string Contact { get; }
        ContactType Type { get; }
    }

    public enum ContactType
    {
        Phone,
        Email,
        WebSite
    }
}
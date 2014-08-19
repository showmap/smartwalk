using SmartWalk.Shared;

namespace SmartWalk.Server.Records
{
    public class ContactRecord
    {
        [UsedImplicitly]
        public virtual int Id { get; set; }
        public virtual EntityRecord EntityRecord { get; set; }
        public virtual byte Type { get; set; }
        public virtual string Title { get; set; }
        public virtual string Contact { get; set; }
        public virtual bool IsDeleted { get; set; }
    }

    public enum ContactType
    {
        Email = 0,
        Url = 1,
        Phone = 2
    }
}
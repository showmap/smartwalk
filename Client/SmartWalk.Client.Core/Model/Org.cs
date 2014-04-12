using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Model.DataContracts;

namespace SmartWalk.Client.Core.Model
{
    public class Org
    {
        private readonly Entity _entity;

        public Org(Entity entity)
        {
            _entity = entity;
        }

        public Entity Info
        {
            get { return _entity; }
        }

        public OrgEvent[] OrgEvents { get; set; }

        public override bool Equals(object obj)
        {
            var org = obj as Org;
            if (org != null)
            {
                return Info.Id == org.Info.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                    .CombineHashCode(Info.GetHashCode())
                    .CombineHashCodeOrDefault(OrgEvents);
        }

        public override string ToString()
        {
            return string.Format(
                "Id={0}, EventsCount={1}", 
                Info.Id, 
                OrgEvents != null ? OrgEvents.Length : 0);
        }
    }
}
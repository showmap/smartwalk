using SmartWalk.Core.Utils;

namespace SmartWalk.Core.Model
{
    public class Org : Entity
    {
        public OrgEventInfo[] EventInfos { get; set; }

        public override bool Equals(object obj)
        {
            var org = obj as Org;
            if (org != null)
            {
                return Equals(Info, org.Info) &&
                    Description == org.Description &&
                        EventInfos.EnumerableEquals(org.EventInfos);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCodeOrDefault(Info)
                    .CombineHashCodeOrDefault(Description)
                        .CombineHashCodeOrDefault(EventInfos);
        }
    }
}
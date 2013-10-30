using SmartWalk.Core.Utils;

namespace SmartWalk.Core.Model
{
    public class LocationIndex : EntityInfo
    {
        public EntityInfo[] OrgInfos { get; set; }

        public override bool Equals(object obj)
        {
            var index = obj as LocationIndex;
            if (index != null)
            {
                return base.Equals(index) &&
                    OrgInfos.EnumerableEquals(index.OrgInfos);
            }

            return false;
        }
    }
}
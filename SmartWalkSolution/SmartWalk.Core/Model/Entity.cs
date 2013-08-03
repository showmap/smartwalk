using SmartWalk.Core.Utils;

namespace SmartWalk.Core.Model
{
    public abstract class Entity
    {
        public EntityInfo Info { get; set; }

        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            var entity = obj as Entity;
            if (entity != null)
            {
                return Equals(Info, entity.Info) &&
                    Description == entity.Description;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCodeOrDefault(Info)
                    .CombineHashCodeOrDefault(Description);
        }
    }
}
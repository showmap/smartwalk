using SmartWalk.Shared.Utils;
using SmartWalk.Client.Core.Model.Interfaces;

namespace SmartWalk.Client.Core.Model
{
    public abstract class Entity : ISearchable
    {
        public EntityInfo Info { get; set; }

        public string Description { get; set; }

        public virtual string SearchableText
        {
            get
            {
                return (Info != null ? " " + Info.SearchableText : string.Empty) + 
                    (Description != null ? " " + Description : string.Empty);
            }
        }

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
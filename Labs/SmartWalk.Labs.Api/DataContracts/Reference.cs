using System.Linq;
using SmartWalk.Shared.DataContracts;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Labs.Api.DataContracts
{
    public class Reference : IReference
    {
        public int Id { get; set; }
        public string Storage { get; set; }
        public int? Type { get; set; }

        public override bool Equals(object obj)
        {
            var rf = obj as Reference;
            if (rf != null)
            {
                return 
                    Id == rf.Id &&
                    Storage == rf.Storage &&
                    Type == rf.Type;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCode(Id)
                .CombineHashCodeOrDefault(Storage)
                .CombineHashCode(Type);
        }
    }

	public static class ReferenceExtension 
	{
		public static int Id(this Reference[] refs)
		{
			var smartWalkRef = refs != null 
				? refs.FirstOrDefault(r => r.Storage == Storage.SmartWalk) 
				: null;
			return smartWalkRef != null ? smartWalkRef.Id : 0;
		}
	}
}
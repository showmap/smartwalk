using SmartWalk.Shared.Utils;

namespace SmartWalk.Shared.DataContracts
{
    public class Pictures
    {
        public string Small { get; set; }
        public string Medium { get; set; }
        public string Full { get; set; }

        public override bool Equals(object obj)
        {
            var p = obj as Pictures;
            if (p != null)
            {
                return 
                    Small == p.Small &&
                    Medium == p.Medium &&
                    Full == p.Full;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Initial
                .CombineHashCodeOrDefault(Small)
                .CombineHashCodeOrDefault(Medium)
                .CombineHashCodeOrDefault(Full);
        }
    }
}
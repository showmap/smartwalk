using System.Collections;
using System.Linq;

namespace SmartWalk.Shared.Utils
{
    public static class HashCode
    {
        public const int Initial = 17;
        private const int Multiplier = 23;

        public static int CombineHashCode<T>(this int hashCode, T arg)
        {
            unchecked
            {
                return Multiplier * hashCode + arg.GetHashCode();
            }
        }

        public static int CombineHashCodeOrDefault<T>(this int hashCode, T arg) where T : class
        {
            unchecked
            {
                return Multiplier * hashCode + 
                    (arg == null 
                        ? Initial 
                        : (arg is IEnumerable
                            ? ((IEnumerable)arg).Cast<object>().Aggregate(
                                Initial, 
                                (i1, i2) => i1.GetHashCode().CombineHashCode(i2))
                            : arg.GetHashCode()));
            }
        }
    }
}
using System.Linq;
using System.Collections.Generic;

namespace SmartWalk.Shared.Utils
{
    public static class EnumerableExtensions
    {
        public static bool EnumerableEquals<T>(
            this IEnumerable<T> enumerable1, 
            IEnumerable<T> enumerable2)
        {
            var result = Equals(enumerable1, enumerable2) || 
                (enumerable1 != null && enumerable2 != null && enumerable1.SequenceEqual(enumerable2));
            return result;
        }
    }
}
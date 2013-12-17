using System.Linq;
using System.Collections.Generic;

namespace SmartWalk.Core.Utils
{
    public static class EnumerableExtensions
    {
        public static bool EnumerableEquals<T>(
            this IEnumerable<T> enumerable1, 
            IEnumerable<T> enumerable2)
        {
            return Equals(enumerable1, enumerable2) || 
                (enumerable1 != null && enumerable2 != null && enumerable1.SequenceEqual(enumerable2));
        }
    }
}
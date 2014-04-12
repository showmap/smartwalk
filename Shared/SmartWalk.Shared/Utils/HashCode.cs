namespace SmartWalk.Shared.Utils
{
    public static class HashCode
    {
        public const int Initial = 17;
        public const int Multiplier = 23;

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
                return Multiplier * hashCode + (arg == null ? Initial : arg.GetHashCode());
            }
        }
    }
}
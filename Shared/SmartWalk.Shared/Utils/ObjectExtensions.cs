using System;

namespace SmartWalk.Shared.Utils
{
    public static class ObjectExtensions
    {
        public const double Epsilon = 0.00001;

        public static bool EqualsF(this float left, float right)
        {
            return Math.Abs(left - right) < Epsilon;
        }

        public static bool EqualsF(this double left, float right)
        {
            return EqualsF((float)left, right);
        }

        public static bool EqualsF(this float left, double right)
        {
            return EqualsF(left, (float)right);
        }

        public static bool EqualsF(this double left, double right)
        {
            return EqualsF((float)left, (float)right);
        }
    }
}
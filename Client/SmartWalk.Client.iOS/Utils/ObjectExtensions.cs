using System;

namespace SmartWalk.Client.iOS.Utils
{
    public static class ObjectExtensions
    {
        public static bool EqualsNF(this nfloat left, nfloat right)
        {
            return Math.Abs(left - right) < SmartWalk.Shared.Utils.ObjectExtensions.Epsilon;
        }

        public static bool EqualsNF(this double left, nfloat right)
        {
            return EqualsNF((nfloat)left, right);
        }

        public static bool EqualsNF(this nfloat left, double right)
        {
            return EqualsNF(left, (nfloat)right);
        }

        public static bool EqualsNF(this double left, double right)
        {
            return EqualsNF((nfloat)left, (nfloat)right);
        }
    }
}
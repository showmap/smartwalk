using System;
using Foundation;

namespace SmartWalk.Client.iOS.Utils
{
    public static class ObjectExtensions
    {
        private static readonly DateTime _reference = 
            TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(2001, 1, 1, 0, 0, 0));

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

        public static DateTime ToDateTime(this NSDate date)
        {
            return _reference.AddSeconds(date.SecondsSinceReferenceDate);
        }

        public static NSDate ToNSDate(this DateTime date)
        {
            return NSDate.FromTimeIntervalSinceReferenceDate((date - _reference).TotalSeconds);
        }

        public static NSDate ToNSDate(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToNSDate() : null;
        }
    }
}
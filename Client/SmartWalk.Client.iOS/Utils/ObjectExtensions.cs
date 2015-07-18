using System;
using Foundation;
using UIKit;

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

        public static UIColor GetLighter(this UIColor color, float amount = 0.25f)
        {
            return color.GetColorWithBrightness(1 + amount);
        }

        public static UIColor GetDarker(this UIColor color, float amount = 0.25f)
        {
            return color.GetColorWithBrightness(1 - amount);
        }

        private static UIColor GetColorWithBrightness(this UIColor color, float amount)
        {
            nfloat hue, saturation, brightness, alpha;
            color.GetHSBA(out hue, out saturation, out brightness, out alpha);
            var result = UIColor.FromHSBA(hue, saturation, brightness * amount, alpha);
            return result;
        }
    }
}
using System;
using CoreGraphics;
using Foundation;
using UIKit;

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

        public static DateTime ToDateTime(this NSDate date)
        {
            var secs = date.SecondsSinceReferenceDate;
            if (secs < -63113904000) return DateTime.MinValue;
            if (secs > 252423993599) return DateTime.MaxValue;
            return (DateTime) date;
        }

        public static NSDate ToNSDate(this DateTime date)
        {
            if (date.Kind == DateTimeKind.Unspecified)
            {
                date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            }

            return (NSDate)date;
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

        public static UIImage ToImage(this UIColor color)
        {
            UIGraphics.BeginImageContextWithOptions(new CGSize(1, 1), false, 0);
            color.SetFill();
            UIGraphics.RectFill(new CGRect(0, 0, 1, 1));
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return image;
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
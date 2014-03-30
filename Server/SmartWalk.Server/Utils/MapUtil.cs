using System.Drawing;
using System.Linq;

namespace SmartWalk.Server.Utils
{
    public static class MapUtil
    {
        public static PointF GetMiddleCoordinate(PointF[] coordinates)
        {
            if (coordinates.Length == 0) return PointF.Empty;

            var result = new RectangleF(coordinates.First(), SizeF.Empty);

            foreach (var coordinate in coordinates)
            {
                if (coordinate != PointF.Empty)
                {
                    var rect = new RectangleF(coordinate, SizeF.Empty);
                    result = RectangleF.Union(result, rect);
                }
            }

            return new PointF(result.X + result.Width / 2, result.Y + result.Height / 2);
        }
    }
}
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartWalk.Server.Utils;

namespace SmartWalk.Server.UnitTests.UtilTests
{
    [TestClass]
    public class MapUtilTest
    {
        [TestMethod]
        public void GetMiddleCoordinateRealTest()
        {
            var coordinates = new[]
                {
                    new PointF(37.754237f, -122.412065f),
                    new PointF(37.754113f, -122.414268f),
                    new PointF(37.75256f, -122.412923f),
                    new PointF(37.760708f, -122.410678f),
                    new PointF(37.755351f, -122.419876f)
                };

            var middle = MapUtil.GetMiddleCoordinate(coordinates);

            Assert.AreEqual(new PointF(37.7566338f, -122.415276f), middle);
        }

        [TestMethod]
        public void GetMiddleCoordinateSimpleTest()
        {
            var coordinates = new[]
                {
                    new PointF(5, 5),
                    new PointF(10, 10),
                    new PointF(20, 20)
                };

            var middle = MapUtil.GetMiddleCoordinate(coordinates);

            Assert.AreEqual(new PointF(12.5f, 12.5f), middle);
        }

        [TestMethod]
        public void GetMiddleCoordinateEmptyTest()
        {
            var coordinates = new PointF[] {};

            var middle = MapUtil.GetMiddleCoordinate(coordinates);

            Assert.AreEqual(PointF.Empty, middle);
        }
    }
}
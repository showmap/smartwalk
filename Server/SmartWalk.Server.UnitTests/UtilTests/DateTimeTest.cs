using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.UnitTests.UtilTests
{
    [TestClass]
    public class DateTimeTest
    {
        [TestMethod]
        public void InDayTest()
        {
            var day = new DateTime(2014, 11, 29);

            var time1 = new DateTime(2014, 11, 29, 5, 20, 0);
            Assert.IsFalse(time1.IsTimeThisDay(day)); // 'cause it's before 6am

            var time2 = new DateTime(2014, 11, 29, 18, 20, 0);
            Assert.IsTrue(time2.IsTimeThisDay(day));
        }

        [TestMethod]
        public void InDayTestWithHalfRange()
        {
            var day = new DateTime(2014, 11, 29);

            var range1 = new Tuple<DateTime, DateTime?>(day, null);
            var time1 = new DateTime(2014, 11, 29, 5, 20, 0);
            Assert.IsTrue(time1.IsTimeThisDay(day, range1)); // 'cause it's before 6am but first day

            var range2 = new Tuple<DateTime, DateTime?>(day.AddDays(-2), null);
            Assert.IsFalse(time1.IsTimeThisDay(day, range2)); // 'cause it's before 6am and not first day
        }

        [TestMethod]
        public void InDayTestWithFullRange()
        {
            var day = new DateTime(2014, 11, 29);

            var range1 = new Tuple<DateTime, DateTime?>(day.AddDays(-2), day.AddDays(2));
            var time1 = new DateTime(2014, 11, 29, 5, 20, 0);
            Assert.IsFalse(time1.IsTimeThisDay(day, range1)); // 'cause it's before 6am

            var time2 = new DateTime(2014, 11, 29, 18, 20, 0);
            Assert.IsTrue(time2.IsTimeThisDay(day, range1));
        }

        [TestMethod]
        public void InDayTestOutsideOfFullRange()
        {
            var day = new DateTime(2014, 11, 29);

            var range1 = new Tuple<DateTime, DateTime?>(day.AddDays(-2), day.AddDays(2));
            var time1 = new DateTime(2014, 11, 20, 5, 20, 0);
            Assert.IsFalse(time1.IsTimeThisDay(day, range1)); // 'cause it's outside range and it's not first day now
            Assert.IsTrue(time1.IsTimeThisDay(day.AddDays(-2), range1)); // the day has all ahead times

            var time2 = new DateTime(2014, 12, 15, 18, 20, 0);
            Assert.IsFalse(time2.IsTimeThisDay(day, range1));
            Assert.IsTrue(time2.IsTimeThisDay(day.AddDays(2), range1));
        }
    }
}
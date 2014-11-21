using System;
using System.Linq;
using NUnit.Framework;
using SmartWalk.Client.Core.Model.DataContracts;
using SmartWalk.Client.Core.Utils;

namespace SmartWalk.Client.UnitTests.UtilTests
{
    [TestFixture]
    public class ModelTest
    {
        [Test]
        public void GroupByDayNoShowsTest()
        {
            var shows = default(Show[]);
            var groups = shows.GroupByDay();

            Assert.AreEqual(null, groups);
        }

        [Test]
        public void GroupByDayAllShowsWithTime()
        {
            var shows = new [] {
                new Show { StartTime = DateTime.Today.Date.AddHours(15) },
                new Show { StartTime = DateTime.Today.Date.AddHours(16) },
                new Show { StartTime = DateTime.Today.Date.AddDays(1).AddHours(15) },
                new Show { StartTime = DateTime.Today.Date.AddDays(2).AddHours(15) },
            };

            var groups = shows.GroupByDay();

            Assert.AreEqual(3, groups.Count);

            var keys = groups.Keys.ToArray();
            Assert.AreEqual(DateTime.Today.Date, keys[0]);
            Assert.AreEqual(DateTime.Today.Date.AddDays(1), keys[1]);
            Assert.AreEqual(DateTime.Today.Date.AddDays(2), keys[2]);

            Assert.AreEqual(2, groups[keys[0]].Length);
            Assert.AreEqual(1, groups[keys[1]].Length);
            Assert.AreEqual(1, groups[keys[2]].Length);
        }

        [Test]
        public void GroupByDaySomeShowsWithTime()
        {
            var shows = new [] {
                new Show { StartTime = DateTime.Today.Date.AddHours(15) },
                new Show(),
                new Show { StartTime = DateTime.Today.Date.AddDays(1).AddHours(13) },
                new Show(),
                new Show { StartTime = DateTime.Today.Date.AddDays(2).AddHours(16) },
            };

            var groups = shows.GroupByDay();

            Assert.AreEqual(4, groups.Count);

            var keys = groups.Keys.ToArray();
            Assert.AreEqual(DateTime.MinValue, keys[0]);
            Assert.AreEqual(DateTime.Today.Date, keys[1]);
            Assert.AreEqual(DateTime.Today.Date.AddDays(1), keys[2]);
            Assert.AreEqual(DateTime.Today.Date.AddDays(2), keys[3]);

            Assert.AreEqual(2, groups[keys[0]].Length);
            Assert.AreEqual(1, groups[keys[1]].Length);
            Assert.AreEqual(1, groups[keys[2]].Length);
            Assert.AreEqual(1, groups[keys[3]].Length);
        }

        [Test]
        public void GroupByDayNoShowsWithTime()
        {
            var shows = new [] {
                new Show(),
                new Show(),
                new Show(),
            };

            var groups = shows.GroupByDay();

            Assert.AreEqual(1, groups.Count);

            var keys = groups.Keys.ToArray();
            Assert.AreEqual(DateTime.MinValue, keys[0]);

            Assert.AreEqual(3, groups[keys[0]].Length);
        }
    }
}
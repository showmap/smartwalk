using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartWalk.Shared.Utils;

namespace SmartWalk.Server.UnitTests.UtilTests
{
    [TestClass]
    public class ReflectionTest
    {
        [TestMethod]
        public void TestHasProperty()
        {
            var foo = new FooClass();
            Assert.IsTrue(foo.HasProperty("FooProperty"));
        }

        [TestMethod]
        public void TestHasMethod()
        {
            var foo = new FooClass();
            Assert.IsTrue(foo.HasMethod("FooMethod"));
        }

        [TestMethod]
        public void TestGetValue()
        {
            var foo = new FooClass { FooProperty = "moo" };
            Assert.AreEqual("moo", foo.GetValue<string>("FooProperty"));
        }

        [TestMethod]
        public void TestGetElementType()
        {
            var foo1 = new FooClass { FooProperty = "moo" };
            var foo2 = new FooClass { FooProperty = "moo" };
            var foo3 = new FooClass { FooProperty = "moo" };
            var coll = new[] { foo1, foo2, foo3 };
            var elemType = coll.GetType().GetElementType();
            Assert.AreEqual(typeof(FooClass), elemType);
        }
    }
}

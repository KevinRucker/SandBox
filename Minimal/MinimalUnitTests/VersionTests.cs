using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimal.Utility;

namespace MinimalUnitTests
{
    [TestClass]
    public class Version_Test
    {
        [TestMethod]
        public void ApplicationVersionTest()
        {
            var version = Minimal.Utility.Version.ApplicationVersion();
            Assert.AreEqual("1.0.0", version);
        }

        [TestMethod]
        public void LibraryVersionTest()
        {
            var version = Minimal.Utility.Version.LibraryVersion();
            Assert.AreEqual("1.0.0", version);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimal.Common;
using Minimal.Interfaces;
using System;
using System.Collections.Generic;

namespace MinimalUnitTests
{
    [TestClass]
    public class BinaryHeaderUnitTest
    {
        [TestMethod]
        public void BinaryHeader_ConstructorTest()
        {
            var header = BinaryHeader.Factory();
            Assert.IsNotNull(header);
        }

        [TestMethod]
        public void BinaryHeader_ParameterizedConstructorTest()
        {
            var TestEntries = new List<IHeaderEntry>();
            TestEntries.Add(HeaderEntry.Factory("entry1", (byte)1));
            TestEntries.Add(HeaderEntry.Factory("entry2", (short)2));
            TestEntries.Add(HeaderEntry.Factory("entry3", (int)3));
            var header = BinaryHeader.Factory(TestEntries);
            Assert.IsNotNull(header);
            Assert.AreEqual((byte)1, header["entry1"].EntryValue, "entry1 check failed.");
            Assert.AreEqual((short)2, header["entry2"].EntryValue, "entry2 check failed.");
            Assert.AreEqual((int)3, header["entry3"].EntryValue, "entry3 check failed.");
        }

        [TestMethod]
        public void BinaryHeader_FunctionalTest()
        {
            var TestEntries1 = new List<IHeaderEntry>();
            TestEntries1.Add(HeaderEntry.Factory("entry1", (byte)1));
            TestEntries1.Add(HeaderEntry.Factory("entry2", (short)2));
            TestEntries1.Add(HeaderEntry.Factory("entry3", (int)3));
            var header1 = (BinaryHeader)BinaryHeader.Factory(TestEntries1);
            var bytes1 = new byte[header1.Size];
            bytes1 = header1.HeaderBytes();

            var TestEntries2 = new List<IHeaderEntry>();
            TestEntries2.Add(HeaderEntry.Factory("entry1", default(byte)));
            TestEntries2.Add(HeaderEntry.Factory("entry2", default(short)));
            TestEntries2.Add(HeaderEntry.Factory("entry3", default(int)));
            var header2 = (BinaryHeader)BinaryHeader.Factory(bytes1, TestEntries2);
            var bytes2 = new byte[header2.Size];
            bytes2 = header2.HeaderBytes();

            var entry1 = (byte)header2["entry1"].EntryValue;
            var entry2 = (short)header2["entry2"].EntryValue;
            var entry3 = (int)header2["entry3"].EntryValue;

            Assert.AreEqual(Convert.ToBase64String(bytes1), Convert.ToBase64String(bytes2));
        }
    }
}

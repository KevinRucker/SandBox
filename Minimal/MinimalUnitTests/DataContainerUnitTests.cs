using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimal.Common;
using Minimal.Interfaces;
using System;
using System.Collections.Generic;

namespace MinimalUnitTests
{
    [TestClass]
    public class DataContainerUnitTests
    {
        [TestMethod]
        public void DataContainer_ConstructorTest()
        {
            var container = DataContainer.Factory();
            Assert.IsNotNull(container);
        }

        [TestMethod]
        public void DataContainer_FactoryTest()
        {
            // Create Header Definition
            var headerdef = new List<IHeaderEntry>();
            headerdef.Add(HeaderEntry.Factory("entry1", default(byte)));
            headerdef.Add(HeaderEntry.Factory("entry2", default(byte)));
            headerdef.Add(HeaderEntry.Factory("entry3", default(byte)));
            headerdef.Add(HeaderEntry.Factory("entry4", default(byte)));

            // Create BinaryHeader and add values
            var header = BinaryHeader.Factory(headerdef);
            header["entry1"].EntryValue = (byte)1;
            header["entry2"].EntryValue = (byte)2;
            header["entry3"].EntryValue = (byte)3;
            header["entry4"].EntryValue = (byte)4;

            // Sample data
            byte[] data1 = { 5, 6, 7, 8, 9, 10 };

            // Create DataContainer, add Header and Data
            var container = DataContainer.Factory();
            container.Header = header;
            container.Data = data1;

            // This creates a single byte[] containing the header and data
            var test1 = container.GetBytes();

            // Create new DataContainer from the byte[] test1 and the Header Definition
            var container2 = DataContainer.Factory(test1, headerdef);

            Assert.AreEqual(Convert.ToBase64String(data1), Convert.ToBase64String(container2.Data), "Data check failed.");
            Assert.AreEqual(Convert.ToBase64String(header.HeaderBytes()), Convert.ToBase64String(container2.Header.HeaderBytes()), "HeaderBytes check failed.");
        }

        [TestMethod]
        public void DataContainer_FunctionalTest()
        {
            var expected = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Create Header Definition
            var headerdef = new List<IHeaderEntry>();
            headerdef.Add(HeaderEntry.Factory("entry1", default(byte)));
            headerdef.Add(HeaderEntry.Factory("entry2", default(byte)));
            headerdef.Add(HeaderEntry.Factory("entry3", default(byte)));
            headerdef.Add(HeaderEntry.Factory("entry4", default(byte)));

            // Create BinaryHeader and add values
            var header = BinaryHeader.Factory(headerdef);
            header["entry1"].EntryValue = (byte)1;
            header["entry2"].EntryValue = (byte)2;
            header["entry3"].EntryValue = (byte)3;
            header["entry4"].EntryValue = (byte)4;

            // Sample data
            byte[] data1 = { 5, 6, 7, 8, 9, 10 };
            var testcd1 = Convert.ToBase64String(data1);

            // Create DataContainer, add Header and Data
            var container = DataContainer.Factory();
            container.Header = header;
            container.Data = data1;

            // This creates a single byte[] containing the header and data
            var test1 = container.GetBytes();

            Assert.AreEqual(Convert.ToBase64String(expected), Convert.ToBase64String(test1));
        }
    }
}

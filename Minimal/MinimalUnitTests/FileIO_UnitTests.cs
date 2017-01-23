using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimal.Utility;

namespace MinimalUnitTests
{
    [TestClass]
    public class FileIO_UnitTests
    {
        private string _binFileName = "Test-" + Guid.NewGuid().ToString() + ".bin";
        private string _textFileName = "Test-" + Guid.NewGuid().ToString() + ".txt";

        [TestMethod]
        public void FileIO_WriteDeleteBinaryFileTest()
        {
            var content = TestDataFactory.GetLoremIpsumBytes(0x400);
            FileIO.WriteBinaryFile(_binFileName, System.IO.FileMode.CreateNew, content);
            Assert.IsTrue(System.IO.File.Exists(_binFileName), "WriteBinaryFile failed.");
            FileIO.Delete(_binFileName, false);
            Assert.IsFalse(System.IO.File.Exists(_binFileName), "Delete failed.");
        }

        [TestMethod]
        public void FileIO_WriteDeleteTextFileTest()
        {
            var content = TestDataFactory.GetLoremIpsumString(0x400, new System.Text.UTF8Encoding());
            FileIO.WriteTextFile(_textFileName, System.IO.FileMode.CreateNew, content);
            Assert.IsTrue(System.IO.File.Exists(_textFileName), "WriteTextFile failed.");
            FileIO.Delete(_textFileName, false);
            Assert.IsFalse(System.IO.File.Exists(_textFileName), "Delete failed.");
        }

        [TestMethod]
        public void FileIO_SecureDeleteUnitTEst()
        {
            var content = TestDataFactory.GetLoremIpsumBytes(0x400);
            FileIO.WriteBinaryFile(_binFileName, System.IO.FileMode.CreateNew, content);
            Assert.IsTrue(System.IO.File.Exists(_binFileName), "WriteBinaryFile failed.");
            FileIO.Delete(_binFileName, true);
            Assert.IsFalse(System.IO.File.Exists(_binFileName), "Secure Delete failed.");
        }
    }
}

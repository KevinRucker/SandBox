using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimal.Compression;
using Minimal.Interfaces;
using System;

namespace MinimalUnitTests
{
    [TestClass]
    public class CompressionProviderUnitTest
    {
        private string _origin_string = string.Empty;
        private byte[] _origin_bytes = null;
        private int _size = 0x400; // 1KB

        [TestInitialize]
        public void TestInit()
        {
            if (string.IsNullOrEmpty(_origin_string))
            {
                _origin_string = TestDataFactory.GetLoremIpsumString(_size, new System.Text.UTF8Encoding());
                _origin_bytes = TestDataFactory.GetLoremIpsumBytes(_size);
            }
        }

        [TestMethod]
        public void CompressionProvider_ConstructorTest()
        {
            ICompressionProvider cp = null;
            cp = CompressionProvider.Factory();
            Assert.IsNotNull(cp);
        }

        [TestMethod]
        public void CompressionProvider_CompressDecompressStringTest()
        {
            var origin_size = _origin_string.Length;
            var cp = CompressionProvider.Factory();
            var target1 = cp.CompressString(_origin_string);
            var compressed_size = target1.Length;
            var target2 = cp.DecompressString(target1);
            Assert.IsTrue(origin_size > compressed_size, "Compressed size is greater than uncompressed size.");
            Assert.AreEqual(_origin_string, target2, "Decompressed string was not equal to original string.");
        }

        [TestMethod]
        public void CompressionProvider_CompressDecompressBytesTest()
        {
            var origin_size = _origin_bytes.Length;
            var cp = CompressionProvider.Factory();
            var target1 = cp.CompressBytes(_origin_bytes);
            var compressed_size = target1.Length;
            var target2 = cp.DecompressBytes(target1);
            Assert.IsTrue(origin_size > compressed_size, "Compressed size is greater than uncompressed size.");
            Assert.AreEqual(Convert.ToBase64String(_origin_bytes), Convert.ToBase64String(target2), "Decompressed byte array was not equal to original byte array.");
        }
    }
}

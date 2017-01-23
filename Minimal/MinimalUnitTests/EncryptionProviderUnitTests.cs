using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimal.Common;
using Minimal.Security.Encryption;
using System.Text;

namespace MinimalUnitTests
{
    [TestClass]
    public class EncryptionProviderUnitTests
    {
        private string origin = default(string);
        private string passphrase = default(string);

        [TestInitialize]
        public void TestInit()
        {
            origin = TestDataFactory.GetFillerText();
            passphrase = "t3stP4ss!";
        }

        [TestMethod]
        public void AESEncryptionProvider_ConstructorTest()
        {
            var ep = AESEncryptionProvider.Factory();
            Assert.IsNotNull(ep);
        }

        [TestMethod]
        public void AESEncryptionProvider_StringsTest()
        {
            var ep = AESEncryptionProvider.Factory();
            var target1 = ep.EncryptString(passphrase, origin);
            var target2 = ep.DecryptString(passphrase, target1);
            Assert.AreNotEqual(origin, target1, "AES EncryptString failed.");
            Assert.AreEqual(origin, target2, "AES DecryptString failed.");
        }

        [TestMethod]
        public void AESEncryptionProvider_BytesTest()
        {
            var test = Factory.CreateBuffer(new UTF8Encoding().GetByteCount(origin));
            test = new UTF8Encoding().GetBytes(origin);
            var ep = AESEncryptionProvider.Factory();
            var target1 = ep.EncryptBytes(passphrase, test);
            var target2 = ep.DecryptBytes(passphrase, target1);
            var final = new UTF8Encoding().GetString(target2);
            Assert.AreEqual(origin, final);
        }

        [TestMethod]
        public void TripleDESEncryptionProvider_ConstructorTest()
        {
            var ep = TripleDESEncryptionProvider.Factory();
            Assert.IsNotNull(ep);
        }

        [TestMethod]
        public void TripleDESEncryptionProvider_StringsTest()
        {
            var ep = TripleDESEncryptionProvider.Factory();
            var target1 = ep.EncryptString(passphrase, origin);
            var target2 = ep.DecryptString(passphrase, target1);
            Assert.AreNotEqual(origin, target1, "TripleDES EncryptString failed.");
            Assert.AreEqual(origin, target2, "TripleDES DecryptString failed.");
        }

        [TestMethod]
        public void TripleDESEncryptionProvider_BytesTest()
        {
            var test = Factory.CreateBuffer(new UTF8Encoding().GetByteCount(origin));
            test = new UTF8Encoding().GetBytes(origin);
            var ep = TripleDESEncryptionProvider.Factory();
            var target1 = ep.EncryptBytes(passphrase, test);
            var target2 = ep.DecryptBytes(passphrase, target1);
            var final = new UTF8Encoding().GetString(target2);
            Assert.AreEqual(origin, final);
        }
    }
}

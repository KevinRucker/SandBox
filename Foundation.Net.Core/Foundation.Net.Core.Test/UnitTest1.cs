using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Foundation.Net.Core.Encryption;

namespace Foundation.Net.Core.Test
{
    [TestClass]
    public class UnitTest1
    {
        private string testData = "Now is the time for all good men to come to the aid of their country.";
        private string passphrase = "This is the test passphrase";

        [TestMethod]
        public void TestMethod1()
        {
            var crypto = new AesEncryptionProvider();
            var encrypted = crypto.EncryptString(passphrase, testData);
            var decrypted = crypto.DecryptString(passphrase, encrypted);
            Assert.AreEqual(testData, decrypted);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var crypto = new AesEncryptionProvider();
            var encrypted = crypto.EncryptBytes(passphrase, new UTF8Encoding().GetBytes(testData));
            var decrypted = new UTF8Encoding().GetString(crypto.DecryptBytes(passphrase, encrypted));
            Assert.AreEqual(testData, decrypted);
        }
    }
}

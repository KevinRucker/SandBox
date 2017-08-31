using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Foundation.Net.Core.Encryption;

namespace Foundation.Net.Core.Test
{
    [TestClass]
    public class AesCryptoTests
    {
        private string testData = "Now is the time for all good men to come to the aid of their country.";
        private string passphrase = "This is the test passphrase";
        private int keySize = 256; // Legal values for Aes are 128, 192, and 256

        [TestMethod]
        public void EncryptDecryptString()
        {
            var crypto = new AesEncryptionProvider();
            var encrypted = crypto.EncryptString(passphrase, testData, keySize);
            var decrypted = crypto.DecryptString(passphrase, encrypted, keySize);
            Assert.AreEqual(testData, decrypted);
        }

        [TestMethod]
        public void EncryptDecryptStringFail()
        {
            var crypto = new AesEncryptionProvider();
            var encrypted = crypto.EncryptString(passphrase, testData, keySize);
            Assert.ThrowsException<System.Security.Cryptography.CryptographicException>(
                () => crypto.DecryptString("Different passphrase", encrypted, keySize));
            Assert.ThrowsException<System.ArgumentException>(
                () => crypto.DecryptString(passphrase, encrypted, keySize - 4));
        }

        [TestMethod]
        public void EncryptDecryptBytes()
        {
            var crypto = new AesEncryptionProvider();
            var encrypted = crypto.EncryptBytes(passphrase, new UTF8Encoding().GetBytes(testData), keySize);
            var decrypted = new UTF8Encoding().GetString(crypto.DecryptBytes(passphrase, encrypted, keySize));
            Assert.AreEqual(testData, decrypted);
        }

        [TestMethod]
        public void EncryptDecryptBytesFail()
        {
            var crypto = new AesEncryptionProvider();
            var encrypted = crypto.EncryptBytes(passphrase, new UTF8Encoding().GetBytes(testData), keySize);
            Assert.ThrowsException<System.Security.Cryptography.CryptographicException>(
                () => new UTF8Encoding().GetString(crypto.DecryptBytes("Different passphrase", encrypted, keySize)));
            Assert.ThrowsException<System.ArgumentException>(
                () => crypto.DecryptBytes(passphrase, encrypted, keySize - 4));
        }
    }
}

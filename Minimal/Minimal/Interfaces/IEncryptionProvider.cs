using System.Text;

namespace Minimal.Interfaces
{
    /// <summary>
    /// Interface for EncryptionProvider implementations
    /// </summary>
    public interface IEncryptionProvider
    {
        /// <summary>
        /// Returns <c>true</c> if encryption algorithm is certified by NIST as FIPS 140-2 compliant; otherwise <c>false</c>
        /// </summary>
        bool IsNISTCertifiedAlgorithm { get; }
        /// <summary>
        /// Encrypts a <code>System.String</code> using the provided passphrase to generate the Key and IV
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.String</code> value to encrypt</param>
        /// <returns>base64 encoded encrypted <code>System.String</code></returns>
        string EncryptString(string passphrase, string value);
        /// <summary>
        /// Encrypts a <code>System.String</code> using the provided passphrase to generate the Key and IV
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.String</code> value to encrypt</param>
        /// <param name="encoder">The <code>System.Text.Encoding</code> used to convert string to byte[]. Pass null to use standard encoding (UTF8).</param>
        /// <returns>base64 encoded encrypted <code>System.String</code></returns>
        string EncryptString(string passphrase, string value, Encoding encoder);
        /// <summary>
        /// Decrypts an encrypted, base64 encoded <code>System.String</code> using the provided passphrase to generate the Key and IV
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value">base64 encoded, encrypted <code>System.String</code> to decrypt</param>
        /// <returns>Decrypted <code>System.String</code></returns>
        string DecryptString(string passphrase, string value);
        /// <summary>
        /// Decrypts an encrypted, base64 encoded <code>System.String</code> using the provided passphrase to generate the Key and IV
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value">base64 encoded, encrypted <code>System.String</code> to decrypt</param>
        /// <param name="encoder">The <code>System.Text.Encoding</code> used to convert byte[] to string. Pass null to use standard encoding (UTF8).</param>
        /// <returns>Decrypted <code>System.String</code></returns>
        string DecryptString(string passphrase, string value, Encoding encoder);
        /// <summary>
        /// Encrypts a <code>System.byte[]</code> using the provided passphrase to generate the Key and IV
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.byte[]</code> data to encrypt</param>
        /// <returns>Encrypted <code>System.byte[]</code></returns>
        byte[] EncryptBytes(string passphrase, byte[] value);
        /// <summary>
        /// Decrypts an encrypted <code>System.byte[]</code>
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.byte[]</code> data to decrypt</param>
        /// <returns>Decrypted <code>System.byte[]</code></returns>
        byte[] DecryptBytes(string passphrase, byte[] value);
    }
}

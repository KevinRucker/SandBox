using System.Text;

namespace Minimal.Interfaces
{
    /// <summary>
    /// Interface for CompressionProvider implementations
    /// </summary>
    public interface ICompressionProvider
    {
        /// <summary>
        /// Compress a <code>string</code> value
        /// </summary>
        /// <param name="value"><code>string</code> to compress</param>
        /// <returns><code>byte[]</code></returns>
        byte[] CompressString(string value);
        /// <summary>
        /// Compress a <code>string</code> value
        /// </summary>
        /// <param name="value"><code>string</code> to compress</param>
        /// <param name="encoder">The <code>System.Text.Encoding</code> used to convert string to byte[]. Pass null to use standard encoding (UTF8).</param>
        /// <returns><code>byte[]</code></returns>
        byte[] CompressString(string value, Encoding encoder);
        /// <summary>
        /// Decompress a <code>byte[]</code>
        /// </summary>
        /// <param name="value">The <code>byte[]</code> to decompress.</param>
        /// <returns><code>string</code></returns>
        string DecompressString(byte[] value);
        /// <summary>
        /// Decompress a <code>byte[]</code>
        /// </summary>
        /// <param name="value">The <code>byte[]</code> to decompress.</param>
        /// <param name="encoder">The <code>System.Text.Encoding</code> used to convert byte[] to string. Pass null to use standard encoding (UTF8).</param>
        /// <returns><code>string</code></returns>
        string DecompressString(byte[] value, Encoding encoder);
        /// <summary>
        /// Compress <code>byte[]</code>
        /// </summary>
        /// <param name="value"><code>byte[]</code> to compress</param>
        /// <returns><code>byte[]</code></returns>
        byte[] CompressBytes(byte[] value);
        /// <summary>
        /// Decompress <code>byte[]</code>
        /// </summary>
        /// <param name="value"><code>byte[]</code> to decompress</param>
        /// <returns><code>byte[]</code></returns>
        byte[] DecompressBytes(byte[] value);
    }
}

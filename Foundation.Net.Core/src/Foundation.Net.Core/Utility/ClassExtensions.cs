using System;

namespace Foundation.Net.Core.Utility
{
    public static class ClassExtensions
    {
        /// <summary>
        /// Block encryption algorithms pad the end of the plain data, if necessary, to make the data
        /// length a multiple of the algorithm's block size (exanple: 16 bytes in the case of AES).
        /// The decryption algorithm does not remove these padding bytes. This <code>byte[]</code>
        /// extension method removes the padding bytes if necessary.
        /// </summary>
        /// <param name="padValue"><code>System.byte</code> containing pad value</param>
        /// <param name="scanSize"><code>System.int</code> containing scan size</param>
        /// <returns>Trimmed <code>System.byte[]</code></returns>
        public static byte[] TrimPaddingBytes(this byte[] origin, byte padValue, int scanSize)
        {
            // Scan the last scanSize bytes of the origin data for padding bytes
            var count = 0;
            for (var i = origin.Length - 1; i > origin.Length - scanSize - 1; i--)
            {
                if (origin[i] == padValue)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            // Remove any padding bytes, if detected and return trimmed data
            if (count != 0)
            {
                var buffer = new byte[origin.Length - count];
                Buffer.BlockCopy(origin, 0, buffer, 0, buffer.Length);
                return buffer;
            }

            // No padding detected return origin data
            return origin;
        }
    }
}

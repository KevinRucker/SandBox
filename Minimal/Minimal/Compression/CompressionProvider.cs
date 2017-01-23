// Author: Kevin Rucker
// License: BSD 3-Clause
// Copyright (c) 2014 - 2015, Kevin Rucker
// All rights reserved.

// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
// 3. Neither the name of the copyright holder nor the names of its contributors
//    may be used to endorse or promote products derived from this software without
//    specific prior written permission.
//
// Disclaimer:
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using Minimal.Custom_Exceptions;
using Minimal.Interfaces;
using Minimal.Utility;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Minimal.Compression
{
    /// <summary>
    /// Concrete implementation of CompressionProvider class
    /// </summary>
    public class CompressionProvider : ICompressionProvider
    {
        /// <summary>
        /// Private constructor prevents direct instantiation (Factory pattern)
        /// </summary>
        private CompressionProvider()
        {

        }

        /// <summary>
        /// Factory method
        /// </summary>
        /// <returns>Object implementing ICompressionProvider</returns>
        public static ICompressionProvider Factory()
        {
            return new CompressionProvider();
        }

        /// <summary>
        /// Compress a <code>string</code> value using standard encoder (utf8)
        /// </summary>
        /// <param name="value"><code>string</code> to compress</param>
        /// <returns><code>byte[]</code></returns>
        public byte[] CompressString(string value)
        {
            return CompressString(value, null);
        }

        /// <summary>
        /// Compress a <code>string</code> value
        /// </summary>
        /// <param name="value"><code>string</code> to compress</param>
        /// <param name="encoder">The <code>System.Text.Encoding</code> used to convert string to byte[]. Pass null to use standard encoding (UTF8).</param>
        /// <returns><code>byte[]</code></returns>
        public byte[] CompressString(string value, Encoding encoder)
        {
            try
            {
                if (encoder == null) { encoder = Common.Factory.GetStandardEncoder; }
                return CompressBytes(encoder.GetBytes(value));
            }
            catch (Exception ex)
            {
                throw new CompressionException("CompressionProvider::compressString unable to compress string.", ex);
            }
        }

        /// <summary>
        /// Decompress a <code>byte[]</code> using standard encoder (utf8)
        /// </summary>
        /// <param name="value">The <code>byte[]</code> to decompress.</param>
        /// <returns><code>string</code></returns>
        public string DecompressString(byte[] value)
        {
            return DecompressString(value, null);
        }

        /// <summary>
        /// Decompress a <code>byte[]</code>
        /// </summary>
        /// <param name="value">The <code>byte[]</code> to decompress.</param>
        /// <param name="encoder">The <code>System.Text.Encoding</code> used to convert byte[] to string. Pass null to use standard encoding (UTF8).</param>
        /// <returns><code>string</code></returns>
        public string DecompressString(byte[] value, Encoding encoder)
        {
            try
            {
                if (encoder == null) { encoder = Common.Factory.GetStandardEncoder; }
                return Common.Factory.EncodeBytes(encoder, DecompressBytes(value));
            }
            catch (Exception ex)
            {
                throw new CompressionException("CompressionProvider::decompressString unable to decompress string.", ex);
            }
        }

        /// <summary>
        /// Compress <code>byte[]</code>
        /// </summary>
        /// <param name="value"><code>byte[]</code> to compress</param>
        /// <returns><code>byte[]</code></returns>
        public byte[] CompressBytes(byte[] value)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var zStream = new DeflateStream(ms, CompressionLevel.Optimal, false))
                    {
                        zStream.Write(value, 0, value.Length);
                        zStream.Close();
                    }
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new CompressionException("CompressionProvider::compressBytes unable to compress byte[].", ex);
            }
        }

        /// <summary>
        /// Decompress <code>byte[]</code>
        /// </summary>
        /// <param name="value"><code>byte[]</code> to decompress</param>
        /// <returns><code>byte[]</code></returns>
        public byte[] DecompressBytes(byte[] value)
        {
            try
            {
                using (var ms = new MemoryStream(value))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    using (var zStream = new DeflateStream(ms, CompressionMode.Decompress, false))
                    {
                        return StreamUtility.ReadFullStream(zStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new CompressionException("CompressionProvider::decompressBytes unable to decompress byte[].", ex);
            }
        }
    }
}

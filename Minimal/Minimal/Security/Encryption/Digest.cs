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

using Minimal.Common;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Minimal.Security.Encryption
{
    /// <summary>
    /// Implementation of cryptographic digest
    /// </summary>
    public static class Digest
    {
        private static readonly object _lock = new object();
        private static Encoding encoder = Factory.GetStandardEncoder;

        private static Func<string, Encoding, Int32> getByteCount = (x, y) => y.GetByteCount(x);

        /// <summary>
        /// Generates encryption key from passphrase
        /// </summary>
        /// <param name="PassPhrase">Passphrase to use</param>
        /// <param name="size">Number of bytes in generated key</param>
        /// <returns><code>byte[]</code> containing encryption key</returns>
        public static byte[] getKeyFromPassPhrase(string PassPhrase, Int32 size)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                var buffer = Factory.CreateBuffer(getByteCount(PassPhrase, encoder));
                buffer = Factory.DecodeString(encoder, PassPhrase);
                return getDigest(buffer, size);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Generates initialization vector (IV) from passphrase
        /// </summary>
        /// <param name="PassPhrase">Passphrase to use</param>
        /// <param name="size">Number of bytes in generated IV</param>
        /// <returns><code>byte[]</code> containing IV</returns>
        public static byte[] getIVFromPassPhrase(string PassPhrase, Int32 size)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                var buffer = Factory.CreateBuffer(getByteCount(PassPhrase, encoder));
                buffer = Factory.DecodeString(encoder, PassPhrase);
                Array.Reverse(buffer);
                return getDigest(buffer, size);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Generates encryption digest
        /// </summary>
        /// <param name="value">Base value, object must implement <see cref="System.Runtime.Serialization.ISerializable"/></param>
        /// <param name="digestLength">Requested number of bytes in digest</param>
        /// <returns><code>byte[]</code> containing encryption digest</returns>
        public static byte[] getDigest<T>(T value, Int32 digestLength)
            where T : System.Runtime.Serialization.ISerializable
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                var bf = new BinaryFormatter();
                var ms = new MemoryStream();
                bf.Serialize(ms, value);
                return getDigest(ms.ToArray(), digestLength);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Generates encryption digest
        /// </summary>
        /// <param name="value">Base value</param>
        /// <param name="digestLength">Requested number of bytes in digest</param>
        /// <returns><code>byte[]</code> containing encryption digest</returns>
        public static byte[] getDigest(byte[] value, Int32 digestLength)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                var iterations = 0;
                // Find first non-zero byte value to use to calculate iterations
                for (var i = 0; i < value.Length; i++)
                {
                    if (value[i] != 0) { iterations = (Int32)(value[i] * 10); break; }
                }
                // There were no non-zero byte values use the max for iterations
                if (iterations == 0) { iterations = (Int32)(byte.MaxValue * 10); }
                var deriveBytes = new Rfc2898DeriveBytes(value, new SHA256CryptoServiceProvider().ComputeHash(value), iterations);
                return deriveBytes.GetBytes(digestLength);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }
    }
}

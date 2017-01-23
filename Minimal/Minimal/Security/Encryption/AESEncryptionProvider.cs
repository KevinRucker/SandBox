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
using Minimal.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Minimal.Security.Encryption
{
    /// <summary>
    /// Concrete implementation of AES256 EncryptionProvider
    /// </summary>
    public class AESEncryptionProvider : IEncryptionProvider
    {
        /// <summary>
        /// Private constructor prevents direct instantiation (Factory pattern)
        /// </summary>
        private AESEncryptionProvider()
        {

        }

        /// <summary>
        /// Factory method
        /// </summary>
        /// <returns>Object implementing IEncryptionProvider</returns>
        public static IEncryptionProvider Factory()
        {
            return new AESEncryptionProvider();
        }

        /// <summary>
        /// Returns <c>true</c> if encryption algorithm is certified by NIST as FIPS 140-2 compliant; otherwise <c>false</c>
        /// </summary>
        public bool IsNISTCertifiedAlgorithm
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Encrypts a <code>System.String</code> using the provided passphrase to generate the Key and IV and the standard encoder (utf8)
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.String</code> value to encrypt</param>
        /// <returns>base64 encoded encrypted <code>System.String</code></returns>
        public string EncryptString(string passphrase, string value)
        {
            return EncryptString(passphrase, value, null);
        }

        /// <summary>
        /// Encrypts a <code>System.String</code> using the provided passphrase to generate the Key and IV
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.String</code> value to encrypt</param>
        /// <param name="encoder">The <code>System.Text.Encoding</code> used to convert string to byte[]. Pass null to use standard encoding (UTF8).</param>
        /// <returns>base64 encoded encrypted <code>System.String</code></returns>
        public string EncryptString(string passphrase, string value, Encoding encoder)
        {
            if (encoder == null) { encoder = Common.Factory.GetStandardEncoder; }
            return Common.Factory.BytesToBase64(EncryptBytes(passphrase, Common.Factory.DecodeString(encoder, value)));
        }

        /// <summary>
        /// Decrypts an encrypted, base64 encoded <code>System.String</code> using the provided passphrase to generate the Key and IV and the standard encoder (utf8)
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value">base64 encoded, encrypted <code>System.String</code> to decrypt</param>
        /// <returns>Decrypted <code>System.String</code></returns>
        public string DecryptString(string passphrase, string value)
        {
            return DecryptString(passphrase, value, null);
        }

        /// <summary>
        /// Decrypts an encrypted, base64 encoded <code>System.String</code> using the provided passphrase to generate the Key and IV
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value">base64 encoded, encrypted <code>System.String</code> to decrypt</param>
        /// <param name="encoder">The <code>System.Text.Encoding</code> used to convert byte[] to string. Pass null to use standard encoding (UTF8).</param>
        /// <returns>Decrypted <code>System.String</code></returns>
        public string DecryptString(string passphrase, string value, Encoding encoder)
        {
            if (encoder == null) { encoder = Common.Factory.GetStandardEncoder; }
            return Common.Factory.EncodeBytes(encoder, DecryptBytes(passphrase, Common.Factory.Base64ToBytes(value)));
        }

        /// <summary>
        /// Encrypts a <code>System.byte[]</code> using the provided passphrase to generate the Key and IV
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.byte[]</code> data to encrypt</param>
        /// <returns>Encrypted <code>System.byte[]</code></returns>
        public byte[] EncryptBytes(string passphrase, byte[] value)
        {
            using (var CSP = new AesCryptoServiceProvider())
            {
                using (var Encryptor = CSP.CreateEncryptor(
                    Digest.getKeyFromPassPhrase(passphrase, CSP.KeySize / 8),
                    Digest.getIVFromPassPhrase(passphrase, CSP.BlockSize / 8)))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, Encryptor, CryptoStreamMode.Write))
                        {
                            csEncrypt.Write(value, 0, value.Length);
                            csEncrypt.FlushFinalBlock();
                            var container = DataContainer.Factory();
                            var headerdef = new List<IHeaderEntry>();
                            headerdef.Add(HeaderEntry.Factory("OriginalDataSize", value.Length));
                            container.Header = BinaryHeader.Factory(headerdef);
                            container.Data = msEncrypt.ToArray();
                            return container.GetBytes();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts an encrypted <code>System.byte[]</code>
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.byte[]</code> data to decrypt</param>
        /// <returns>Decrypted <code>System.byte[]</code></returns>
        public byte[] DecryptBytes(string passphrase, byte[] value)
        {
            using (var CSP = new AesCryptoServiceProvider())
            {
                using (var Decryptor = CSP.CreateDecryptor(
                    Digest.getKeyFromPassPhrase(passphrase, CSP.KeySize / 8),
                    Digest.getIVFromPassPhrase(passphrase, CSP.BlockSize / 8)))
                {
                    var headerdef = new List<IHeaderEntry>();
                    headerdef.Add(HeaderEntry.Factory("OriginalDataSize", default(int)));
                    var container = DataContainer.Factory(value, headerdef);
                    using (var msDecrypt = new MemoryStream(container.Data))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, Decryptor, CryptoStreamMode.Read))
                        {
                            var buffer = Common.Factory.CreateBuffer((int)msDecrypt.Length);
                            csDecrypt.Read(buffer, 0, buffer.Length);
                            var result = Common.Factory.CreateBuffer((int)container.Header["OriginalDataSize"].EntryValue);
                            Buffer.BlockCopy(buffer, 0, result, 0, result.Length);
                            return result;
                        }
                    }
                }
            }
        }
    }
}

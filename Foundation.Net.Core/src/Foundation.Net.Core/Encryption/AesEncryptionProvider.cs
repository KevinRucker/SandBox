// Author: Kevin Rucker
// License: BSD 3-Clause
// Copyright (c) 2014 - 2017, Kevin Rucker
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

using Foundation.Net.Core.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Net.Core.Encryption
{
    public class AesEncryptionProvider : IEncryptionProvider
    {
        /// <summary>
        /// Decrypts an encrypted <code>System.byte[]</code>
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.byte[]</code> data to decrypt</param>
        /// <returns>Decrypted <code>System.byte[]</code></returns>
        public byte[] DecryptBytes(string passphrase, byte[] value)
        {
            using (var aes = Aes.Create())
            {
                // Extract IV and data from value
                var iv = (byte[])Array.CreateInstance(typeof(byte), aes.IV.Length);
                Buffer.BlockCopy(value, 0, iv, 0, iv.Length);
                var data = (byte[])Array.CreateInstance(typeof(byte), value.Length - iv.Length);
                Buffer.BlockCopy(value, iv.Length, data, 0, data.Length);
                // Derive Key from passphrase
                aes.Key = new Digest().GetDigest(new UTF8Encoding().GetBytes(passphrase), 32);
                // Decrypt
                using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
                {
                    using (var msDecrypt = new MemoryStream(data))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var resultStream = new MemoryStream())
                            {
                                csDecrypt.CopyTo(resultStream);
                                return TrimPadding(resultStream.ToArray());
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts a base64 encoded <code>System.String</code> using the provided passphrase to generate the Key
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.String</code> value to decrypt</param>
        /// <returns>>Decrypted <code>System.byte[]</code></returns>
        public string DecryptString(string passphrase, string value)
        {
            return new UTF8Encoding().GetString(DecryptBytes(passphrase, Convert.FromBase64String(value)));
        }

        /// <summary>
        /// Encrypts a <code>System.byte[]</code> using the provided passphrase to generate the Key
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.byte[]</code> data to encrypt</param>
        /// <returns>Encrypted <code>System.byte[]</code></returns>
        public byte[] EncryptBytes(string passphrase, byte[] value)
        {
            using (var aes = Aes.Create())
            {
                // Generate IV
                aes.GenerateIV();
                // Derive Key from passphrase
                aes.Key = new Digest().GetDigest(new UTF8Encoding().GetBytes(passphrase), 32);
                // Encrypt
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var resultStream = new MemoryStream())
                    {
                        using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (var plainStream = new MemoryStream(value))
                            {
                                plainStream.CopyTo(aesStream);
                                aesStream.FlushFinalBlock();
                                // Append IV to encrypted data
                                var encrypted = resultStream.ToArray();
                                var buffer = (byte[])Array.CreateInstance(typeof(byte), aes.IV.Length + encrypted.Length);
                                Buffer.BlockCopy(aes.IV, 0, buffer, 0, aes.IV.Length);
                                Buffer.BlockCopy(encrypted, 0, buffer, aes.IV.Length, encrypted.Length);
                                return buffer;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Encrypts a <code>System.String</code> using the provided passphrase to generate the Key
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.String</code> value to encrypt</param>
        /// <returns>base64 encoded encrypted <code>System.String</code></returns>
        public string EncryptString(string passphrase, string value)
        {
            return Convert.ToBase64String(EncryptBytes(passphrase, new UTF8Encoding().GetBytes(value)));
        }

        /// <summary>
        /// Block encryption algorithms pad the end of the plain data if necessary to make the data
        /// length a multiple of the algorithm's block size (16 bytes in the case of AES). The decryption
        /// algorithm does not remove these padding bytes (0). This method removes the padding bytes if
        /// necessary.
        /// </summary>
        /// <param name="data"><code>System.byte[]</code> containing decrypted value</param>
        /// <returns>Trimmed <code>System.byte[]</code></returns>
        private byte[] TrimPadding(byte[] data)
        {
            var count = 0;
            for (var i = data.Length - 1; i > data.Length - (Aes.Create().BlockSize / 8) - 1; i--)
            {
                if (data[i] == 0)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            if (count != 0)
            {
                var buffer = new byte[data.Length - count];
                Buffer.BlockCopy(data, 0, buffer, 0, buffer.Length);
                return buffer;
            }

            return data;
        }
    }
}

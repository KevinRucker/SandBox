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
using Foundation.Net.Core.Utility;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Net.Core.Encryption
{
    public class AesEncryptionProvider : IEncryptionProvider
    {
        /// <summary>
        /// Decrypts an encrypted <code>System.byte[]</code> using the provided passphrase to generate the Key
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.byte[]</code> data to decrypt</param>
        /// <param name="keySize">Key size to use</param>
        /// <returns>Decrypted <code>System.byte[]</code></returns>
        public byte[] DecryptBytes(string passphrase, byte[] value, int keySize)
        {
            using (var aes = Aes.Create())
            {
                if (!aes.VerifySymmetricKeySize(keySize))
                {
                    throw new ArgumentException("Invalid key size specified for Aes provider.");
                }

                // Extract IV and data from value
                var iv = (byte[])Array.CreateInstance(typeof(byte), aes.IV.Length);
                Buffer.BlockCopy(value, 0, iv, 0, iv.Length);
                var data = (byte[])Array.CreateInstance(typeof(byte), value.Length - iv.Length);
                Buffer.BlockCopy(value, iv.Length, data, 0, data.Length);
                // Derive Key from passphrase
                aes.Key = new Digest().GetDigest(new UTF8Encoding().GetBytes(passphrase), keySize / 8);
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
                                return resultStream
                                    .ToArray()
                                    .TrimPaddingBytes(0, aes.BlockSize / 8);
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
        /// <param name="keySize">Key size to use</param>
        /// <returns>>Decrypted <code>System.byte[]</code></returns>
        public string DecryptString(string passphrase, string value, int keySize)
        {
            return new UTF8Encoding().GetString(DecryptBytes(passphrase, Convert.FromBase64String(value), keySize));
        }

        /// <summary>
        /// Encrypts a <code>System.byte[]</code> using the provided passphrase to generate the Key
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.byte[]</code> data to encrypt</param>
        /// <param name="keySize">Key size to use</param>
        /// <returns>Encrypted <code>System.byte[]</code></returns>
        public byte[] EncryptBytes(string passphrase, byte[] value, int keySize)
        {
            using (var aes = Aes.Create())
            {
                if (!aes.VerifySymmetricKeySize(keySize))
                {
                    throw new ArgumentException("Invalid key size specified for Aes provider.");
                }

                // Generate IV
                aes.GenerateIV();
                // Derive Key from passphrase
                aes.Key = new Digest().GetDigest(new UTF8Encoding().GetBytes(passphrase), keySize / 8);
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
                                // Prepend IV to encrypted data
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
        /// <param name="keySize">Key size to use</param>
        /// <returns>base64 encoded encrypted <code>System.String</code></returns>
        public string EncryptString(string passphrase, string value, int keySize)
        {
            return Convert.ToBase64String(EncryptBytes(passphrase, new UTF8Encoding().GetBytes(value), keySize));
        }
    }
}

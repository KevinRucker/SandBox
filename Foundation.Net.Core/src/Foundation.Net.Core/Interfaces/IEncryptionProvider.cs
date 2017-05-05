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

using System.Text;

namespace Foundation.Net.Core.Interfaces
{
    /// <summary>
    /// Interface for EncryptionProvider implementations
    /// </summary>
    public interface IEncryptionProvider
    {
        /// <summary>
        /// Encrypts a <code>System.String</code> using the provided passphrase to generate the Key
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value"><code>System.String</code> value to encrypt</param>
        /// <returns>base64 encoded encrypted <code>System.String</code></returns>
        string EncryptString(string passphrase, string value);
        /// <summary>
        /// Decrypts an encrypted, base64 encoded <code>System.String</code> using the provided passphrase to generate the Key
        /// </summary>
        /// <param name="passphrase"><code>System.String</code> passphrase to use</param>
        /// <param name="value">base64 encoded, encrypted <code>System.String</code> to decrypt</param>
        /// <returns>Decrypted <code>System.String</code></returns>
        string DecryptString(string passphrase, string value);
        /// <summary>
        /// Encrypts a <code>System.byte[]</code> using the provided passphrase to generate the Key
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

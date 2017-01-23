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

using System;
using System.Text;
using System.Web;

namespace Minimal.Common
{
    /// <summary>
    /// Library object factory
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Create a buffer (array of bytes)
        /// </summary>
        public static Func<Int32, byte[]> CreateBuffer = x => (byte[])Array.CreateInstance(typeof(byte), x);

        /// <summary>
        /// Convert byte[] to base64 string
        /// </summary>
        public static Func<byte[], string> BytesToBase64 = x => Convert.ToBase64String(x);

        /// <summary>
        /// convert base64 string to byte[]
        /// </summary>
        public static Func<string, byte[]> Base64ToBytes = x => Convert.FromBase64String(x);

        /// <summary>
        /// Encode byte[] into string using passed Encoding
        /// </summary>
        public static Func<Encoding, byte[], string> EncodeBytes = (x, y) => x.GetString(y);

        /// <summary>
        /// Decode string to byte[] using passed Encoding
        /// </summary>
        public static Func<Encoding, string, byte[]> DecodeString = (x, y) => x.GetBytes(y);

        /// <summary>
        /// Gets standard encoder (UTF8)
        /// </summary>
        public static Encoding GetStandardEncoder
        {
            get
            {
                return new UTF8Encoding();
            }
        }

        /// <summary>
        /// <c>true</c> if appllication is a web application; otherwise <c>false</c>.
        /// </summary>
        public static bool IsWebApp
        {
            get
            {
                return HttpContext.Current != null;
            }
        }
    }
}

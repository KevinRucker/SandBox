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
using System.Text;
using System.Threading;

namespace Minimal.Utility
{
    /// <summary>
    /// Utility methods for reading Streams.
    /// </summary>
    public static class StreamUtility
    {
        private static readonly object _lock = new object();
        private const Int32 _standardBufferSize = 0x10000; //64k
        private static Encoding _standardEncoding = Factory.GetStandardEncoder;

        /// <summary>
        /// Reads a <code>System.IO.Stream</code> to it's end using default (64K) buffer.
        /// </summary>
        /// <param name="stream"><code>System.IO.Stream</code> to read.</param>
        /// <returns><code>System.byte[]</code></returns>
        /// <seealso cref="ReadFullStream(Stream, Int32)"/>
        public static byte[] ReadFullStream(Stream stream)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                return ReadFullStream(stream, _standardBufferSize);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Reads a <code>System.IO.Stream</code> to it's end using indicated buffer size.
        /// </summary>
        /// <param name="stream"><code>System.IO.Stream</code></param>
        /// <param name="bufferSize">Buffer size in bytes</param>
        /// <returns><code>System.byte[]</code></returns>
        /// <seealso cref="ReadFullStream(Stream)"/>
        public static byte[] ReadFullStream(Stream stream, Int32 bufferSize)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                if (bufferSize <= 0)
                {
                    bufferSize = _standardBufferSize;
                }

                var bytesRead = 0;
                var endOfStream = false;
                var buffer = Factory.CreateBuffer(bufferSize);

                using (var ms = new MemoryStream())
                {
                    while (!endOfStream)
                    {
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            ms.Write(buffer, 0, bytesRead);
                        }
                        else
                        {
                            endOfStream = true;
                        }
                    }
                    return ms.ToArray();
                }
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Reads <code>System.String</code> contents into a <code>System.IO.Stream</code> using default (UTF8) encoding.
        /// </summary>
        /// <param name="value"><code>System.String</code></param>
        /// <returns><code>System.IO.Stream</code></returns>
        /// <seealso cref="StringToStream(string, Encoding)"/>
        public static Stream StringToStream(string value)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                return StringToStream(value, _standardEncoding);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Reads <code>System.String</code> contents into a <code>System.IO.Stream</code> using indicated <code>System.Text.Encoding</code>.
        /// </summary>
        /// <param name="value"><code>System.String</code></param>
        /// <param name="encoder"><code>System.Text.Encoding</code></param>
        /// <returns><code>System.String</code></returns>
        /// <seealso cref="StringToStream(string)"/>
        public static Stream StringToStream(string value, Encoding encoder)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                return (Stream)(new MemoryStream(encoder.GetBytes(value)));
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Reads System.IO.Stream contents into a System.String using default (UTF8) encoding.
        /// </summary>
        /// <param name="stream">System.IO.Stream</param>
        /// <returns>System.String</returns>
        /// <seealso cref="StreamToString(Stream, Encoding)"/>
        public static string StreamToString(Stream stream)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                return StreamToString(stream, _standardEncoding);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Reads System.IO.Stream contents into a System.String using indicated encoding.
        /// </summary>
        /// <param name="stream">System.IO.Stream</param>
        /// <param name="encoder">System.Text.Encoding</param>
        /// <returns>System.String</returns>
        /// <seealso cref="StreamToString(Stream)"/>
        public static string StreamToString(Stream stream, Encoding encoder)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                return encoder.GetString(ReadFullStream(stream));
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }
    }
}

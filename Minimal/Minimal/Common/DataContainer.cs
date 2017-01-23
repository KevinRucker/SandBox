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

using Minimal.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace Minimal.Common
{
    /// <summary>
    /// Container for binary data including <see cref="BinaryHeader"/>.
    /// </summary>
    public class DataContainer : IDataContainer
    {
        private IBinaryHeader _header = BinaryHeader.Factory();
        private byte[] _data;

        /// <summary>
        /// Private default constructor
        /// </summary>
        private DataContainer()
        {

        }

        /// <summary>
        /// Factory method
        /// </summary>
        /// <returns>Empty <see cref="DataContainer"/></returns>
        public static IDataContainer Factory()
        {
            return new DataContainer();
        }

        /// <summary>
        /// Factory method, constructs DataContainer from data and <see cref="BinaryHeader"/>.
        /// </summary>
        /// <param name="data">Binary data</param>
        /// <param name="headerdef"><see cref="BinaryHeader"/> definition.</param>
        /// <returns>DataContainer containing provided <see cref="BinaryHeader"/> and data.</returns>
        public static IDataContainer Factory(byte[] data, IList<IHeaderEntry> headerdef)
        {
            var container = Factory();
            container.Header = BinaryHeader.Factory(headerdef);
            var buffer = Common.Factory.CreateBuffer(container.Header.Size);
            var ms = new MemoryStream(data);
            ms.Read(buffer, 0, buffer.Length);
            container.Header = BinaryHeader.Factory(buffer, headerdef);
            buffer = Common.Factory.CreateBuffer(data.Length - container.Header.Size);
            ms.Read(buffer, 0, data.Length - container.Header.Size);
            container.Data = buffer;
            return container;
        }

        /// <summary>
        /// <see cref="BinaryHeader"/>
        /// </summary>
        public IBinaryHeader Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
            }
        }

        /// <summary>
        /// Binary data
        /// </summary>
        public byte[] Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        /// <summary>
        /// Returns binary representation of <see cref="DataContainer"/>
        /// </summary>
        /// <returns><code>byte[]</code></returns>
        public byte[] GetBytes()
        {
            var buffer = Common.Factory.CreateBuffer(_header.Size + _data.Length);
            Buffer.BlockCopy(_header.HeaderBytes(), 0, buffer, 0, _header.Size);
            Buffer.BlockCopy(_data, 0, buffer, _header.Size, _data.Length);
            return buffer;
        }
    }
}

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
using System.Linq;

namespace Minimal.Common
{
    /// <summary>
    /// Binary Header implementation
    /// </summary>
    public class BinaryHeader : IBinaryHeader
    {
        private IList<IHeaderEntry> _entrymap = new List<IHeaderEntry>();

        /// <summary>
        /// Private Constructor
        /// </summary>
        private BinaryHeader()
        {

        }

        /// <summary>
        /// Private Constructor
        /// </summary>
        /// <param name="entryGraph">Graph of objects which implement <see cref="IHeaderEntry"/></param>
        private BinaryHeader(IList<IHeaderEntry> entryGraph)
        {
            _entrymap = entryGraph;
        }

        /// <summary>
        /// Private Constructor
        /// </summary>
        /// <param name="bytes"><code>byte[]</code> containing a <see cref="BinaryHeader"/></param>
        /// <param name="entryGraph">Graph of objects which implement <see cref="IHeaderEntry"/></param>
        private BinaryHeader(byte[] bytes, IList<IHeaderEntry> entryGraph)
        {
            _entrymap = entryGraph;
            var ms = new MemoryStream(bytes);
            foreach (var entry in _entrymap)
            {
                var temp = Common.Factory.CreateBuffer(entry.EntrySize);
                ms.Read(temp, 0, entry.EntrySize);
                entry.EntryBytes = temp;
            }
        }

        /// <summary>
        /// Object factory method
        /// </summary>
        /// <returns><code>BinaryHeader</code> instance</returns>
        public static IBinaryHeader Factory()
        {
            return new BinaryHeader();
        }

        /// <summary>
        /// Object factory method
        /// </summary>
        /// <param name="entryGraph">Graph of objects which implement <see cref="IHeaderEntry"/></param>
        /// <returns><code>BinaryHeader</code> instance</returns>
        public static IBinaryHeader Factory(IList<IHeaderEntry> entryGraph)
        {
            return new BinaryHeader(entryGraph);
        }

        /// <summary>
        /// Object factory method
        /// </summary>
        /// <param name="bytes"><code>byte[]</code> containing a <see cref="BinaryHeader"/></param>
        /// <param name="entryGraph">Graph of objects which implement <see cref="IHeaderEntry"/></param>
        /// <returns><code>BinaryHeader</code> instance</returns>
        public static IBinaryHeader Factory(byte[] bytes, IList<IHeaderEntry> entryGraph)
        {
            return new BinaryHeader(bytes, entryGraph);
        }

        /// <summary>
        /// Add <see cref="HeaderEntry"/> to graph
        /// </summary>
        /// <param name="entry">Object which implements <see cref="IHeaderEntry"/></param>
        public void AddEntry(IHeaderEntry entry)
        {
            _entrymap.Add(entry);
        }

        /// <summary>
        /// Add <see cref="HeaderEntry"/> to graph
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddEntry(string name, object value)
        {
            _entrymap.Add(HeaderEntry.Factory(name, value));
        }

        /// <summary>
        /// Size property
        /// </summary>
        public int Size
        {
            get
            {
                var value = 0;
                foreach (var entry in _entrymap)
                {
                    value += entry.EntrySize;
                }
                return value;
            }
        }

        /// <summary>
        /// Entry count property
        /// </summary>
        public int EntryCount
        {
            get
            {
                return _entrymap.Count();
            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="name"><code>string</code> containing name of desired <see cref="HeaderEntry"/></param>
        /// <returns></returns>
        public IHeaderEntry this[string name]
        {
            get
            {
                var value = from x in _entrymap
                            where x.EntryName == name
                            select x;

                if (value == null)
                {
                    throw new ArgumentException("No HeaderEntry with name " + name + " exists in graph.");
                }
                return ((HeaderEntry)value.FirstOrDefault());
            }
        }

        /// <summary>
        /// Get <see cref="BinaryHeader"/> as an array of bytes
        /// </summary>
        /// <returns><see cref="BinaryHeader"/> as an array of bytes</returns>
        public byte[] HeaderBytes()
        {
            var bytes = new List<byte>();
            foreach (var entry in _entrymap)
            {
                bytes.AddRange(entry.EntryBytes);
            }
            return bytes.ToArray();
        }
    }
}

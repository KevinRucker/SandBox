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
using Minimal.Security.Encryption;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Minimal.StateTracking
{
    /// <summary>
    /// Value state tracking
    /// </summary>
    public class ValueStateTracker<TValue> : IValueStateTracker<TValue>, IChangeTracking
    {
        private byte[] _state = new byte[1] { 0 };
        private byte[] _change = new byte[1] { 0 };
        private TValue _current;

        /// <summary>
        /// Constructor
        /// </summary>
        public ValueStateTracker()
        {
            SetInitialState();
        }

        /// <summary>
        /// Current value
        /// </summary>
        public TValue CurrentValue
        {
            get
            {
                return _current;
            }
        }

        /// <summary>
        /// <c>true</c> if value has changed; otherwise <c>false</c>
        /// </summary>
        public bool IsChanged
        {
            get
            {
                return Convert.ToBase64String(Digest.getDigest(_state, 32)) != Convert.ToBase64String(Digest.getDigest(_change, 32));
            }
        }

        /// <summary>
        /// Change the value
        /// </summary>
        /// <param name="newValue">New value</param>
        public void ChangeValue(TValue newValue)
        {
            if (newValue != null)
            {
                var formatter = new BinaryFormatter();
                var ms = new MemoryStream();
                formatter.Serialize(ms, newValue);
                _change = Digest.getDigest(ms.ToArray(), 32);
                if (Convert.ToBase64String(Digest.getDigest(_state, 32)) != Convert.ToBase64String(Digest.getDigest(_change, 32)))
                {
                    _current = newValue;
                }
            }
            else
            {
                _current = default(TValue);
            }
        }

        /// <summary>
        /// Accept changes
        /// </summary>
        public void AcceptChanges()
        {
            _state = _change;
        }

        /// <summary>
        /// Set initial state
        /// </summary>
        private void SetInitialState()
        {
            ChangeValue(default(TValue));
            AcceptChanges();
        }
    }
}

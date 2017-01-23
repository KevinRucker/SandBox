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
using Minimal.Interfaces;

namespace Minimal.Utility
{
    /// <summary>
    /// Concrete implementation of BitMask, a class for manipulating a set of bits (bit mask)
    /// </summary>
    public class BitMask : IBitMask
    {
        private UInt64 _baseValue = UInt64.MinValue;
        private Int32 _maxBitPosition = (sizeof(UInt64) * 8) - 1;

        /// <summary>
        /// Constructor
        /// </summary>
        private BitMask() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseValue">Base value for BitMask</param>
        private BitMask(UInt64 baseValue) { _baseValue = baseValue; }

        /// <summary>
        /// Factory method
        /// </summary>
        /// <returns>BitMask object</returns>
        public static IBitMask Factory()
        {
            return new BitMask();
        }

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="baseValue">Base value for BitMask</param>
        /// <returns>BitMask object</returns>
        public static IBitMask Factory(UInt64 baseValue)
        {
            return new BitMask(baseValue);
        }

        /// <summary>
        /// The base value of the bitmap
        /// </summary>
        public UInt64 BaseValue { get { return _baseValue; } set { _baseValue = value; } }

        /// <summary>
        /// Get the value of the bit at the supplied position position in the bitmask
        /// </summary>
        /// <param name="bitPosition">Bit position</param>
        /// <returns><code>true</code> if the bit is on and <code>false</code> if it is off</returns>
        public bool GetBitValue(Int32 bitPosition)
        {
            TestBitPosition(bitPosition);
            return (_baseValue & GetMask(bitPosition)) == GetMask(bitPosition);
        }

        /// <summary>
        /// Turn on bit at the supplied position in the bitmask
        /// </summary>
        /// <param name="bitPosition">Bit position</param>
        public void SetBit(Int32 bitPosition)
        {
            TestBitPosition(bitPosition);
            _baseValue |= GetMask(bitPosition);
        }

        /// <summary>
        /// Turn off bit at the supplied position in the bitmask
        /// </summary>
        /// <param name="bitPosition">Bit position</param>
        public void ClearBit(Int32 bitPosition)
        {
            TestBitPosition(bitPosition);
            _baseValue &= ~GetMask(bitPosition);
        }

        /// <summary>
        /// Turn on all bits in the bitmask
        /// </summary>
        public void SetAll()
        {
            _baseValue = UInt64.MaxValue;
        }

        /// <summary>
        /// Turn off all bits in the bitmask
        /// </summary>
        public void ClearAll()
        {
            _baseValue = UInt64.MinValue;
        }

        /// <summary>
        /// Test bit at given position
        /// </summary>
        /// <param name="bitPosition">Position to test</param>
        private void TestBitPosition(Int32 bitPosition)
        {
            if (bitPosition > _maxBitPosition)
            {
                throw new ArgumentOutOfRangeException("bitPosition", bitPosition.ToString() + " is outside the allowed range 0 - " + _maxBitPosition.ToString() + ".");
            }
        }

        /// <summary>
        /// Get the mask value
        /// </summary>
        /// <param name="shift">Position of bit</param>
        /// <returns>Binary mask</returns>
        private static UInt64 GetMask(Int32 shift)
        {
            return (UInt64)0x01 << (shift);
        }
    }
}

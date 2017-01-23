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

namespace Minimal.Common
{
    /// <summary>
    /// Concrete implementation of <see cref="HeaderEntry"/>
    /// </summary>
    public class HeaderEntry : IHeaderEntry
    {
        private string _entryName;
        private object _entryValue;

        /// <summary>
        /// Private default constructor
        /// </summary>
        private HeaderEntry()
        {

        }

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="name">Name of header entry</param>
        /// <param name="value">Value of header entry</param>
        private HeaderEntry(string name, object value)
        {
            _entryName = name;
            _entryValue = value;
        }

        /// <summary>
        /// Factory method
        /// </summary>
        /// <returns><see cref="HeaderEntry"/> instance</returns>
        public static IHeaderEntry Factory()
        {
            return new HeaderEntry();
        }

        /// <summary>
        /// Factory method
        /// </summary>
        /// <param name="name">Name of header entry</param>
        /// <param name="value">Value of header entry</param>
        /// <returns><see cref="HeaderEntry"/> instance</returns>
        public static IHeaderEntry Factory(string name, object value)
        {
            return new HeaderEntry(name, value);
        }

        /// <summary>
        /// Entry value's <see cref="Type"/>
        /// </summary>
        public Type EntryType
        {
            get
            {
                return _entryValue.GetType();
            }
        }

        /// <summary>
        /// Size in bytes of entry (read only)
        /// </summary>
        public int EntrySize
        {
            get
            {
                switch (EntryType.ToString())
                {
                    case "System.Byte":
                        return sizeof(byte);
                    case "System.SByte":
                        return sizeof(sbyte);
                    case "System.Int16":
                        return sizeof(short);
                    case "System.UInt16":
                        return sizeof(ushort);
                    case "System.Int32":
                        return sizeof(int);
                    case "system.UInt32":
                        return sizeof(uint);
                    case "System.Int64":
                        return sizeof(long);
                    case "System.UInt64":
                        return sizeof(ulong);
                    case "System.Char":
                        return sizeof(char);
                    case "System.Single":
                        return sizeof(float);
                    case "System.Double":
                        return sizeof(double);
                    case "System.Decimal":
                        return sizeof(decimal);
                    case "System.Boolean":
                        return sizeof(bool);
                    default:
                        throw new ArgumentException("Only CLR primitive types are allowed.");
                }
            }
        }

        /// <summary>
        /// <code>byte[]</code> containing entry value
        /// </summary>
        public byte[] EntryBytes
        {
            get
            {
                switch (EntryType.ToString())
                {
                    case "System.Byte":
                        return new byte[1] { (byte)EntryValue };
                    case "System.SByte":
                        return new byte[1] { (byte)EntryValue };
                    case "System.Int16":
                        return BitConverter.GetBytes((short)EntryValue);
                    case "System.UInt16":
                        return BitConverter.GetBytes((ushort)EntryValue);
                    case "System.Int32":
                        return BitConverter.GetBytes((int)EntryValue);
                    case "system.UInt32":
                        return BitConverter.GetBytes((uint)EntryValue);
                    case "System.Int64":
                        return BitConverter.GetBytes((long)EntryValue);
                    case "System.UInt64":
                        return BitConverter.GetBytes((ulong)EntryValue);
                    case "System.Char":
                        return BitConverter.GetBytes((char)EntryValue);
                    case "System.Single":
                        return BitConverter.GetBytes((float)EntryValue);
                    case "System.Double":
                        return BitConverter.GetBytes((double)EntryValue);
                    case "System.Decimal":
                        Int32[] bits = decimal.GetBits((decimal)EntryValue);
                        List<byte> bytes = new List<byte>();
                        foreach (Int32 i in bits)
                        {
                            bytes.AddRange(BitConverter.GetBytes(i));
                        }
                        return bytes.ToArray();
                    case "System.Boolean":
                        return BitConverter.GetBytes((bool)EntryValue);
                    default:
                        throw new ArgumentException("Only CLR primitive types are allowed.");
                }
            }
            set
            {
                switch (EntryType.ToString())
                {
                    case "System.Byte":
                        _entryValue = value[0];
                        break;
                    case "System.SByte":
                        _entryValue = (sbyte)(value[0]);
                        break;
                    case "System.Int16":
                        _entryValue = BitConverter.ToInt16(value, 0);
                        break;
                    case "System.UInt16":
                        _entryValue = BitConverter.ToInt16(value, 0);
                        break;
                    case "System.Int32":
                        _entryValue = BitConverter.ToInt32(value, 0);
                        break;
                    case "system.UInt32":
                        _entryValue = BitConverter.ToInt32(value, 0);
                        break;
                    case "System.Int64":
                        _entryValue = BitConverter.ToInt64(value, 0);
                        break;
                    case "System.UInt64":
                        _entryValue = BitConverter.ToInt64(value, 0);
                        break;
                    case "System.Char":
                        _entryValue = BitConverter.ToChar(value, 0);
                        break;
                    case "System.Single":
                        _entryValue = BitConverter.ToSingle(value, 0);
                        break;
                    case "System.Double":
                        _entryValue = BitConverter.ToDouble(value, 0);
                        break;
                    case "System.Decimal":
                        if (value.Length != 16)
                            throw new ArgumentException("A decimal must be created from exactly 16 bytes");
                        Int32[] bits = new Int32[4];
                        for (int i = 0; i <= 15; i += 4)
                        {
                            bits[i / 4] = BitConverter.ToInt32(value, i);
                        }
                        _entryValue = new decimal(bits);
                        break;
                    case "System.Boolean":
                        _entryValue = BitConverter.ToBoolean(value, 0);
                        break;
                    default:
                        throw new ArgumentException("Only CLR primitive types are allowed.");
                }
            }
        }

        /// <summary>
        /// Name of entry
        /// </summary>
        public string EntryName
        {
            get
            {
                return _entryName;
            }
            set
            {
                _entryName = value;
            }
        }

        /// <summary>
        /// Value of entry
        /// </summary>
        public object EntryValue
        {
            get
            {
                return _entryValue;
            }
            set
            {
                _entryValue = value;
            }
        }
    }
}

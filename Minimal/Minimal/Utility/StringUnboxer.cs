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
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Threading;

namespace Minimal.Utility
{
    /// <summary>
    /// Convert string values to specified type
    /// </summary>
    public class StringUnboxer
    {
        private static readonly object _lock = new object();
        private static object _convertedValue = null;

        /// <summary>
        /// Convert <code>string</code> value to object of type T
        /// </summary>
        /// <typeparam name="T"><code>Type</code> to return</typeparam>
        /// <param name="value"><code>string</code> containing value to convert</param>
        /// <returns>Object of type T</returns>
        public static T UnBox<T>(string value)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                // Test to see if value is valid to convert to supplied type
                if (CanUnBox<T>(value))
                {
                    // value is valid, return conversion
                    return (T)_convertedValue;
                }
                else
                {
                    // Conversion not possible with given string data, return default value for supplied type
                    switch (typeof(T).ToString())
                    {
                        // In our case, if the supplied type is System.DateTime, we want to return 
                        // System.Data.SQLTypes.SQLDateTime.MinValue (01/01/1753) instead of
                        // System.DateTime.MinValue (01/01/0001) which is the normal default value
                        case "System.DateTime":
                            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(SqlDateTime.MinValue.ToString());
                        // Return the .NET default value for all other types
                        default:
                            return default(T);
                    }
                }
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Attempt conversion
        /// </summary>
        /// <typeparam name="T"><code>Type</code> to return</typeparam>
        /// <param name="value"><code>string</code> containing value to convert</param>
        /// <returns><code>bool</code> flag indicating conversion succeeded of failed</returns>
        private static bool CanUnBox<T>(string value)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                _convertedValue = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }
    }
}

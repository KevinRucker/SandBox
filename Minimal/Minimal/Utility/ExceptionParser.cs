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
using System.Threading;

namespace Minimal.Utility
{
    /// <summary>
    /// Provides a recursive method to parse nested exceptions into a formatted string.
    /// </summary>
    public class ExceptionParser
    {
        private static readonly object _lock = new object();
        private static string _crlf = Environment.NewLine;
        private const string _tab = "\t";
        private static Int32 _recursionlevel = -1;
        private static StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// Method to parse an exception into a formatted string.
        /// </summary>
        /// <param name="ex">The Exception</param>
        /// <returns>String containing exception detail</returns>
        public static string Parse(Exception ex)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                _sb = new StringBuilder();
                InnerParse(ex);
                return _sb.ToString();
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Recursive method to parse nested exceptions into a formatted string.
        /// </summary>
        /// <param name="ex">The Exception</param>
        private static void InnerParse(Exception ex)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                _recursionlevel++;
                var root = (ex.InnerException == null);
                if (root)
                {
                    _sb.Append("Root Exception:" + _crlf);
                }
                else
                {
                    if (_recursionlevel == 0)
                    {
                        _sb.Append("Outermost Exception:" + _crlf);
                    }
                    else
                    {
                        _sb.Append("Inner Exception [" + _recursionlevel.ToString() + "]:" + _crlf);
                    }
                }
                _sb.Append("Source Module:" + _crlf);
                _sb.Append(_tab + ex.Source + _crlf);
                _sb.Append("Exception Source Method:" + _crlf);
                _sb.Append(_tab + ex.TargetSite + _crlf);
                _sb.Append("Message:" + _crlf);
                _sb.Append(_tab + ex.Message + _crlf);
                _sb.Append("Stack Trace:" + _crlf);
                _sb.Append(ex.StackTrace + _crlf + _crlf);

                if (!root) { InnerParse(ex.InnerException); }
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }
    }
}

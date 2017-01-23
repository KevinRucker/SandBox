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

using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Minimal.Security.Certificates
{
    /// <summary>
    /// 
    /// </summary>
    public static class Certificate
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// Get certificate from store
        /// </summary>
        /// <param name="location">Store location</param>
        /// <param name="IssuerName">Name of certificate issuer</param>
        /// <returns>X509Certificate2</returns>
        public static X509Certificate2 getFromStore(StoreLocation location, string IssuerName)
        {
            try
            {
                Monitor.Enter(_lock);
                X509Store store = new X509Store(location);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
                return collection.Find(X509FindType.FindByIssuerName, IssuerName, true)[0];
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        /// <summary>
        /// Get certificate from pfx file
        /// </summary>
        /// <param name="location">File location</param>
        /// <param name="password">Certificate password</param>
        /// <returns>X509Certificate2</returns>
        public static X509Certificate2 getFromPFXFile(string location, string password)
        {
            try
            {
                Monitor.Enter(_lock);
                return new X509Certificate2(location, password);
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        /// <summary>
        /// Get certificate from cer file
        /// </summary>
        /// <param name="location">File location</param>
        /// <returns>X509Certificate2</returns>
        public static X509Certificate2 getFromCerFile(string location)
        {
            try
            {
                Monitor.Enter(_lock);
                return new X509Certificate2(location);
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }
    }
}

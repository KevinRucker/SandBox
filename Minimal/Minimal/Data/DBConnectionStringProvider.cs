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

using Minimal.Custom_Exceptions;
using Minimal.Interfaces;
using Minimal.Security.Encryption;
using System;
using System.Configuration;

namespace Minimal.Data
{
    /// <summary>
    /// Concrete implementation of class that retrieves connection string from ConnectionStrings section of config file
    /// </summary>
    /// <remarks>
    /// This class allows connection string entries to be encrypted and base64 encoded.
    /// If using encryption, the passphrase for decryption must be present in the appSettings
    /// section of the config file with the key value "ApplicationID".
    /// </remarks>
    public class DBConfigFileCSProvider : IDBConnectionStringProvider
    {
        private string _connectionStringName = string.Empty;
        private bool _encrypted = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ConnectionStringName">Name of ConnectionString in config file</param>
        /// <param name="encrypted"><code>true</code> if ConnectionString value is encrypted</param>
        private DBConfigFileCSProvider(string ConnectionStringName, bool encrypted)
        {
            _connectionStringName = ConnectionStringName;
            _encrypted = encrypted;
        }

        /// <summary>
        /// Static object factory method
        /// </summary>
        /// <param name="ConnectionStringName">Name of ConnectionString in config file</param>
        /// <param name="encrypted"><code>true</code> if ConnectionString value is encrypted</param>
        /// <returns><code>DBConfigFileCSProvider</code> instance</returns>
        public static IDBConnectionStringProvider Factory(string ConnectionStringName, bool encrypted)
        {
            return new DBConfigFileCSProvider(ConnectionStringName, encrypted);
        }

        /// <summary>
        /// Retrieve the ConnectionString
        /// </summary>
        /// <returns><code>System.String</code> The retrieved ConnectionString</returns>
        public string GetConnectionString()
        {
            try
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[_connectionStringName];
                if (settings != null)
                {
                    if (_encrypted)
                    {
                        string ApplicationID = ConfigurationManager.AppSettings["ApplicationID"];
                        if (!string.IsNullOrEmpty(ApplicationID))
                        {
                            IEncryptionProvider crypto = AESEncryptionProvider.Factory();
                            return crypto.DecryptString(ApplicationID, settings.ConnectionString);
                        }
                        else
                        {
                            throw new DBException("DBConfigFileCSProvider::GetConnectionString ApplicationID is missing from appSettings section of config file. Unable to decrypt DB connection string value.");
                        }
                    }
                    else
                    {
                        return settings.ConnectionString;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new DBException("DBConfigFileCSProvider::GetConnectionString Unable to retrieve DB connection string value.", ex);
            }
        }
    }
}

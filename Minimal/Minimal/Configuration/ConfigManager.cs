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
using Minimal.Security.Encryption;
using Minimal.Utility;
using System;
using System.Configuration;
using System.Text;
using System.Threading;

namespace Minimal.Configuration
{
    /// <summary>
    /// Manage access to configuration settings
    /// </summary>
    public static class ConfigManager
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// Get AppSetting value
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="key">Setting key</param>
        /// <returns>Value of Type T</returns>
        public static T AppSetting<T>(string key)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                return AppSetting<T>(key, false);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Get AppSetting value
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="key">Setting key</param>
        /// <param name="encrypted"><c>true</c> if value must be decrypted; otherwise <c>false</c></param>
        /// <returns>Value of Type T</returns>
        public static T AppSetting<T>(string key, bool encrypted)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                var temp = ConfigurationManager.AppSettings[key];
                if (encrypted)
                {
                    var passPhrase = ConfigurationManager.AppSettings["ApplicationId"];
                    if (string.IsNullOrEmpty(passPhrase))
                    {
                        throw new Exception("ConfigManager::AppSetting ApplicationId is missing from the AppSettings section of the config file, unable to decrypt value.");
                    }
                    temp = AESEncryptionProvider.Factory().DecryptString(passPhrase, temp, Factory.GetStandardEncoder);
                }
                return StringUnboxer.UnBox<T>(temp);
            }
            catch (Exception ex)
            {
                throw new Exception("ConfigManager::AppSetting Exception detected.", ex);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Get configuration value
        /// </summary>
        /// <typeparam name="T">Type of return value</typeparam>
        /// <param name="key">Name of configutaion item</param>
        /// <returns>Value</returns>
        public static T ConfigValue<T>(string key)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                var servers = (ConfigSection)ConfigurationManager.GetSection("Environments/Servers");
                var environment = servers[Environment.MachineName.ToLower()];
                if (string.IsNullOrEmpty(environment))
                {
                    environment = servers["Default"];
                }
                if (key.ToLower() == "environment")
                {
                    return StringUnboxer.UnBox<T>(environment);
                }
                var sb = new StringBuilder();
                sb.Append("Environments/");
                sb.Append(environment);
                var cfgData = (ConfigSection)ConfigurationManager.GetSection(sb.ToString());
                return StringUnboxer.UnBox<T>(cfgData[key]);
            }
            catch (Exception ex)
            {
                throw new Exception("ConfigManager::ConfigValue Exception", ex);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }
    }
}

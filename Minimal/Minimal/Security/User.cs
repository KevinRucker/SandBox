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
using Minimal.Custom_Exceptions;
using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

namespace Minimal.Security
{
    /// <summary>
    /// User information
    /// </summary>
    public class User
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// Get user's security identifier
        /// </summary>
        public static string SID
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Request.LogonUserIdentity.User.Value;
                }
                else
                {
                    var tmpIdentity = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                    return GetSid(tmpIdentity.Identity.Name);
                }
            }
        }

        /// <summary>
        /// Get user's network domain
        /// </summary>
        public static string NetworkDomain
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    string winLogin = (!string.IsNullOrEmpty(HttpContext.Current.Request.LogonUserIdentity.Name)) ? HttpContext.Current.Request.LogonUserIdentity.Name : "Anonymous";
                    string[] logon = winLogin.Split('\\');
                    return logon[0];
                }
                else
                {
                    return Dns.GetHostEntry(Dns.GetHostName()).HostName.Replace(Dns.GetHostName() + ".", string.Empty);
                }
            }
        }

        /// <summary>
        /// Determine if user is an administrator on this computer
        /// </summary>
        /// <returns><c>true</c> if the user is an administrator</returns>
        public static bool IsAdmin()
        {
            var identity = WindowsIdentity.GetCurrent();
            if (identity == null) throw new InvalidOperationException("Couldn't get the current user identity");
            var principal = new WindowsPrincipal(identity);

            // Check if this user has the Administrator role. If they do, return immediately.
            // If UAC is on, and the process is not elevated, then this will actually return false.
            if (principal.IsInRole(WindowsBuiltInRole.Administrator)) return true;

            // If we're not running in Vista onwards, we don't have to worry about checking for UAC.
            if (Environment.OSVersion.Platform != PlatformID.Win32NT || Environment.OSVersion.Version.Major < 6)
            {
                // Operating system does not support UAC; skipping elevation check.
                return false;
            }

            int tokenInfLength = Marshal.SizeOf(typeof(int));
            IntPtr tokenInformation = Marshal.AllocHGlobal(tokenInfLength);

            try
            {
                var token = identity.Token;
                var result = NativeMethods.GetTokenInformation(token, NativeMethods.TokenInformationClass.TokenElevationType, tokenInformation, tokenInfLength, out tokenInfLength);

                if (!result)
                {
                    var exception = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
                    throw new InvalidOperationException("Couldn't get token information", exception);
                }

                var elevationType = (NativeMethods.TokenElevationType)Marshal.ReadInt32(tokenInformation);

                switch (elevationType)
                {
                    case NativeMethods.TokenElevationType.TokenElevationTypeDefault:
                        // TokenElevationTypeDefault - User is not using a split token, so they cannot elevate.
                        return false;
                    case NativeMethods.TokenElevationType.TokenElevationTypeFull:
                        // TokenElevationTypeFull - User has a split token, and the process is running elevated. Assuming they're an administrator.
                        return true;
                    case NativeMethods.TokenElevationType.TokenElevationTypeLimited:
                        // TokenElevationTypeLimited - User has a split token, but the process is not running elevated. Assuming they're an administrator.
                        return true;
                    default:
                        // Unknown token elevation type.
                        return false;
                }
            }
            finally
            {
                if (tokenInformation != IntPtr.Zero) Marshal.FreeHGlobal(tokenInformation);
            }
        }

        /// <summary>
        /// PInvoke Windows APIs to determine user's SID
        /// </summary>
        /// <param name="name">Network username</param>
        /// <returns><c>string</c> containing the user's SID</returns>
        private static string GetSid(string name)
        {
            var locked = false;
            try
            {
                Monitor.Enter(_lock, ref locked);
                IntPtr _sid = IntPtr.Zero;
                int _sidLength = 0;
                int _domainLength = 0;
                int _use;
                StringBuilder _domain = new StringBuilder();
                int _error = 0;
                string _sidString = string.Empty;
                // The first call is to determine the amount of memory required to hold the SID
                NativeMethods.LookupAccountName(null, name, _sid, ref _sidLength, _domain, ref _domainLength, out _use);
                _error = Marshal.GetLastWin32Error();
                if (_error != 122) //error 122 (The data area passed to a system call is too small) - normal behavior
                {
                    // Something went wrong...
                    throw (new UserException(new Win32Exception(_error).Message));
                }
                else
                {
                    // Allocate the memory and get the SID
                    _domain = new StringBuilder(_domainLength);
                    _sid = Marshal.AllocHGlobal(_sidLength);
                    bool _rc = NativeMethods.LookupAccountName(null, name, _sid, ref _sidLength, _domain, ref _domainLength, out _use);
                    if (_rc == false)
                    {
                        // Something went wrong...
                        _error = Marshal.GetLastWin32Error();
                        Marshal.FreeHGlobal(_sid);
                        throw (new UserException(new Win32Exception(_error).Message));
                    }
                    else
                    {
                        // convert SID to string
                        _rc = NativeMethods.ConvertSidToStringSid(_sid, ref _sidString);
                        if (_rc == false)
                        {
                            // Something went wrong...
                            _error = Marshal.GetLastWin32Error();
                            Marshal.FreeHGlobal(_sid);
                            throw (new UserException(new Win32Exception(_error).Message));
                        }
                        else
                        {
                            // Free allocated memory and return SID
                            Marshal.FreeHGlobal(_sid);
                            return _sidString;
                        }
                    }
                }
            }
            finally
            {
                if (locked) { Monitor.Exit(_lock); }
            }
        }
    }
}

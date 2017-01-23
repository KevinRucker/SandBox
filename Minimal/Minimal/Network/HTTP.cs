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
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Minimal.Network
{
    /// <summary>
    /// Wrapper class for HTTP functionality
    /// </summary>
    /// <remarks>
    /// Required .Net Framework references:
    /// None
    /// 
    /// Required 3rd Party Assemblies:
    /// None
    /// 
    /// Internal class dependencies:
    /// Minimal.Network.WebHeaderCollectionBuilder
    /// </remarks>
    public static class HTTP
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// Send HTTP request
        /// </summary>
        /// <param name="uri">Destination <code>Uri</code></param>
        /// <param name="verb">HTTP verb to use</param>
        /// <param name="headers">HTTP header collection (null if none)</param>
        /// <param name="certificate">X509 Certificate to use (null if none)</param>
        /// <param name="data">Data to send as body of request (null if none)</param>
        /// <param name="cookies">Cookies to send (null if none)</param>
        /// <returns><code>HttpWebResponse</code></returns>
        public static HttpWebResponse sendRequest(Uri uri, HTTPVerbs verb, WebHeaderCollection headers, X509Certificate2 certificate, byte[] data, CookieCollection cookies)
        {
            try
            {
                Monitor.Enter(_lock);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = verb.ToString();
                if (headers != null)
                {
                    foreach (string header in headers)
                    {
                        switch (header.ToLower())
                        {
                            case "accept":
                                request.Accept = headers["Accept"];
                                break;
                            case "connection":
                                request.Connection = headers["Connection"];
                                break;
                            case "content-type":
                                request.ContentType = headers["Content-type"];
                                break;
                            case "content-Length":
                                request.ContentLength = long.Parse(headers["Content-length"]);
                                break;
                            case "date":
                                request.Date = DateTime.Parse(headers["Date"]);
                                break;
                            case "expect":
                                request.Expect = headers["Expect"];
                                break;
                            case "if-modified-since":
                                request.IfModifiedSince = DateTime.Parse(headers["If-Modified-Since"]);
                                break;
                            case "referer":
                                request.Referer = headers["Referer"];
                                break;
                            case "transfer-encoding":
                                request.TransferEncoding = headers["Transfer-encoding"];
                                break;
                            case "user-agent":
                                request.UserAgent = headers["User-agent"];
                                break;
                            default:
                                request.Headers.Add(header, headers[header]);
                                break;
                        }
                    }
                }
                if (certificate != null)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                    request.ProtocolVersion = HttpVersion.Version10;
                    request.ClientCertificates.Add(certificate);
                }
                if (cookies != null) { request.CookieContainer.Add(cookies); }
                if (data != null && data.Length > 0)
                {
                    request.ContentLength = data.Length;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(data, 0, data.Length);
                    }
                }
                return (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                throw new HTTPException("HTTP::sendRequest Exception encountered sending request.", ex);
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }
    }
}

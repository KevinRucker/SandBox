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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal.Common
{
    /// <summary>
    /// Supported data providers
    /// </summary>
    /// <remarks>
    /// DISCLAIMER: Third party Data Providers must be installed and configured correctly to work.
    /// </remarks>
    public enum DataProviders
    {
        /// <summary>
        /// Microsoft(tm) Odbc (JET) Provider
        /// </summary>
        Odbc,
        /// <summary>
        /// Microsoft(tm) OleDb Provider
        /// </summary>
        OleDb,
        /// <summary>
        /// Microsoft(tm) Oracle Provider (Depreciated after .Net Framework 3.5)
        /// </summary>
        OracleClient,
        /// <summary>
        /// Oracle(tm) Data Provider for .NET (Managed - requires ODAC 12c Release 2)
        /// </summary>
        ODPManaged,
        /// <summary>
        /// Oracle(tm) Data Provider for .NET (Unmanaged)
        /// </summary>
        ODPUnmanaged,
        /// <summary>
        /// FireBird Data Provider
        /// </summary>
        FireBird,
        /// <summary>
        /// Data Provider for PostgreSQL
        /// </summary>
        PostgreSQL,
        /// <summary>
        /// MySQL Data Provider
        /// </summary>
        MySQL,
        /// <summary>
        /// Microsoft (tm) SQL Server Provider
        /// </summary>
        SqlServer
    }

    /// <summary>
    /// Enumeration of HTTP verbs
    /// </summary>
    public enum HTTPVerbs
    {
        /// <summary>
        /// The CONNECT method can be used with a proxy that can dynamically switch to tunneling, as in the case of SSL tunneling. See page 57 of RFC 2616.
        /// </summary>
        CONNECT,
        /// <summary>
        /// The DELETE method requests that a specified URI be deleted. See page 56 of RFC 2616.
        /// </summary>
        DELETE,
        /// <summary>
        /// The GET method retrieves the information or entity that is identified by the URI of the Request. See page 53 of RFC 2616.
        /// </summary>
        GET,
        /// <summary>
        /// The HEAD method is identical to GET except that the server only returns message-headers in the response, without a message-body. See page 54 of RFC 2616
        /// </summary>
        HEAD,
        /// <summary>
        /// The OPTIONS method requests information about the communication options and requirements associated with a URI. See page 52 of RFC 2616.
        /// </summary>
        OPTIONS,
        /// <summary>
        /// The POST method is used to post a new entity as an addition to a URI. The URI identifies an entity that consumes the posted data in some fashion. See page 54 of RFC 2616.
        /// </summary>
        POST,
        /// <summary>
        /// The PUT method is used to replace an entity identified by a URI. See page 55 of RFC 2616.
        /// </summary>
        PUT,
        /// <summary>
        /// The TRACE method invokes a remote, application-layer loop-back of the request message. See page 56 of RFC 2616.
        /// </summary>
        TRACE
    }
}

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

using System.Net.Mail;
using System.IO;

namespace Minimal.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISMTP
    {
        /// <summary>
        /// 
        /// </summary>
        string SMTPServer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        MailAddress From { get; set; }
        /// <summary>
        /// 
        /// </summary>
        System.Int32 AttachmentCount { get; }
        /// <summary>
        /// 
        /// </summary>
        System.Int32 Port { get; set; }
        /// <summary>
        /// 
        /// </summary>
        bool useEmailRelay { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "To")]
        void To(MailAddress address);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void CC(MailAddress value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void BCC(MailAddress value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void Priority(System.Net.Mail.MailPriority value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void Subject(string value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void Body(string value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void isBodyHtml(bool value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void Encoding(System.Text.Encoding value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileSpec"></param>
        void addAttachment(string FileSpec);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisStream"></param>
        /// <param name="Name"></param>
        void addAttachment(Stream thisStream, string Name);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisStream"></param>
        /// <param name="Name"></param>
        /// <param name="ContentType"></param>
        void addAttachment(Stream thisStream, string Name, string ContentType);
        /// <summary>
        /// 
        /// </summary>
        void clearAttachments();
        /// <summary>
        /// 
        /// </summary>
        void Send();
    }
}

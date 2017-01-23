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
using System;
using System.IO;
using System.Net.Mail;

namespace Minimal.Network
{
    /// <summary>
    /// Concrete implementation of the SMTP class
    /// </summary>
    public class SMTP : ISMTP, IDisposable
    {
        private SmtpClient thisSMTP = new SmtpClient();
        private MailMessage thisMail;
        private System.Int32 _Port = 25;
        private bool _useEmailRelay;
        private bool _disposed = default(bool);

        /// <summary>
        /// Private Constructor
        /// </summary>
        private SMTP() { thisMail = new MailMessage(); }

        /// <summary>
        /// Object factory method
        /// </summary>
        /// <returns><code>ISMTP</code> instance</returns>
        public static ISMTP SMTPFactory()
        {
            return new SMTP();
        }

        /// <summary>
        /// <code>System.String</code> containing the SMTP server name
        /// </summary>
        public string SMTPServer { get { return thisSMTP.Host; } set { thisSMTP.Host = value; } }
        /// <summary>
        /// From <code>System.Net.Mail.MailAddress</code>
        /// </summary>
        public MailAddress From { get { return thisMail.From; } set { thisMail.From = value; } }
        /// <summary>
        /// <code>System.Int32</code> Number of attachments
        /// </summary>
        public Int32 AttachmentCount { get { return thisMail.Attachments.Count; } }
        /// <summary>
        /// <code>System.Int32</code> SMTP port
        /// </summary>
        public Int32 Port { get { return thisSMTP.Port; } set { thisSMTP.Port = value; } }
        /// <summary>
        /// <code>System.Boolean</code> indicating use of an email relay
        /// </summary>
        public bool useEmailRelay { get { return _useEmailRelay; } set { _useEmailRelay = value; } }

        /// <summary>
        /// Adds a <code>System.Net.Mail.MailAddress</code> to the To address collection
        /// </summary>
        /// <param name="value"><code>System.Net.Mail.MailAddress</code> to add</param>
        public void To(MailAddress value)
        {
            if (!thisMail.To.Contains(value))
            {
                thisMail.To.Add(value);
            }
        }

        /// <summary>
        /// Adds a <code>System.Net.Mail.MailAddress</code> to the CC address collection
        /// </summary>
        /// <param name="value"><code>System.Net.Mail.MailAddress</code> to add</param>
        public void CC(MailAddress value)
        {
            if (!thisMail.CC.Contains(value))
            {
                thisMail.CC.Add(value);
            }
        }

        /// <summary>
        /// Adds a <code>System.Net.Mail.MailAddress</code> to the BCC address collection
        /// </summary>
        /// <param name="value"><code>System.Net.Mail.MailAddress</code> to add</param>
        public void BCC(MailAddress value)
        {
            if (!thisMail.Bcc.Contains(value))
            {
                thisMail.Bcc.Add(value);
            }
        }

        /// <summary>
        /// Sets the priority
        /// </summary>
        /// <param name="value"><code>System.Net.Mail.MailPriority</code></param>
        public void Priority(MailPriority value)
        {
            thisMail.Priority = value;
        }

        /// <summary>
        /// Message subject
        /// </summary>
        /// <param name="value"><code>System.String</code> containing the message subject</param>
        public void Subject(string value)
        {
            thisMail.Subject = value;
        }

        /// <summary>
        /// Message body
        /// </summary>
        /// <param name="value"><code>System.String</code> containing the message body</param>
        public void Body(string value)
        {
            thisMail.Body = value;
        }

        /// <summary>
        /// Flag indicating HTML content in the message body
        /// </summary>
        /// <param name="value"><code>System.Boolean</code> if true message body contains HTML</param>
        public void isBodyHtml(bool value)
        {
            thisMail.IsBodyHtml = value;
        }

        /// <summary>
        /// Body encoding if body is HTML
        /// </summary>
        /// <param name="value"></param>
        public void Encoding(System.Text.Encoding value)
        {
            thisMail.BodyEncoding = value;
        }

        /// <summary>
        /// Add attachment to email
        /// </summary>
        /// <param name="FileSpec">File to attach</param>
        public void addAttachment(string FileSpec)
        {
            Attachment thisAttachment = new Attachment(FileSpec, System.Net.Mime.MediaTypeNames.Application.Octet);
            thisMail.Attachments.Add(thisAttachment);
        }

        /// <summary>
        /// Add attachment to email
        /// </summary>
        /// <param name="thisStream"><see cref="System.IO.Stream"/> containing content to attach</param>
        /// <param name="Name">Filename for attachment</param>
        public void addAttachment(Stream thisStream, string Name)
        {
            addAttachment(thisStream, Name, System.Net.Mime.MediaTypeNames.Application.Octet);
        }

        /// <summary>
        /// Add attachment to email
        /// </summary>
        /// <param name="thisStream"><see cref="System.IO.Stream"/> containing content to attach</param>
        /// <param name="Name">Filename for attachment</param>
        /// <param name="ContentType">Content (Media) type of attachment</param>
        public void addAttachment(Stream thisStream, string Name, string ContentType)
        {
            Attachment thisAttachment = new Attachment(thisStream, Name, ContentType);
            System.Net.Mime.ContentDisposition Disposition = thisAttachment.ContentDisposition;
            Disposition.FileName = Name;
            Disposition.DispositionType = System.Net.Mime.DispositionTypeNames.Attachment;
            thisMail.Attachments.Add(thisAttachment);
        }

        /// <summary>
        /// Remove all attachments
        /// </summary>
        public void clearAttachments()
        {
            thisMail.Attachments.Clear();
        }

        /// <summary>
        /// Send email to SMTP server
        /// </summary>
        public void Send()
        {
            if (thisMail.To != null & thisMail.From != null)
            {
                if (_useEmailRelay)
                {
                    thisSMTP.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                }
                thisSMTP.Port = _Port;
                thisSMTP.Send(thisMail);
                this.clearAttachments();
            }
            else
            {
                throw new SMTPException("MailMessage not complete");
            }
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation
        /// </summary>
        /// <param name="disposing"><c>true</c> if disposing; otherwise <c>false</c></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    thisSMTP.Dispose();
                    thisSMTP = null;
                    thisMail.Dispose();
                    thisMail = null;
                }
                _disposed = true;
            }
        }
    }
}

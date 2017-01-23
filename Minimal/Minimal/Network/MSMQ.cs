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
using System.Messaging;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace Minimal.Network
{
    /// <summary>
    /// Wrapper class for Microsoft Messaging
    /// </summary>
    /// <remarks>
    /// Required .Net Framework references:
    /// 1. System.Messaging
    /// 2. System.ServiceProcess
    /// 3. System.Web
    /// 
    /// Required 3rd Party Assemblies:
    /// None
    /// </remarks>
    public static class MSMQ
    {
        private static readonly object _lock = new object();
        private static UTF8Encoding _encoder = new UTF8Encoding();

        /// <summary>
        /// Generates a correctly formatted correlation id
        /// </summary>
        public static Func<string> GetCorrelationId = () => Guid.NewGuid().ToString() + @"\" + (new Random((int)DateTime.Now.Ticks).Next(1, 99999)).ToString("0000#");

        /// <summary>
        /// Determine if MSMQ is installed
        /// </summary>
        /// <returns><code>bool</code> indicating if Microsoft Message Queuing is installed</returns>
        /// <remarks>
        /// I normally don't advocate using exception handling in this manner, but...
        /// Microsoft suggests, in the developer blog, to PInvoke the Windows LoadLibrary
        /// API and attempt to load Mqrt.dll. For MSMQ 1.0 and 2.0, the DLL will not
        /// be present and you won't be able to load it. For MSMQ 3.0, the Message
        /// Queuing binaries will be present on the disk, but trying to load the DLL
        /// fails with ERROR_DLL_INIT_FAILED. The following method, although using
        /// exception handling in an unorthodox manner, works, does not require alternate
        /// methodology depending on the MSMQ version, and does not require a PInvoke
        /// call to a Windows API.
        /// </remarks>
        public static bool isMSMQInstalled()
        {
            var lockTaken = false;
            if (!Factory.IsWebApp)
            {
                try
                {
                    Monitor.Enter(_lock, ref lockTaken);
                    using (var sc = new ServiceController("Message Queuing"))
                    {
                        var status = sc.Status;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("was not found on computer"))
                    {
                        return false;
                    }
                    throw new MSMQException("MSMQ::isMSMQInstalled Unexpected exception encountered.", ex);
                }
                finally
                {
                    if (lockTaken) { Monitor.Exit(_lock); }
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Determine if the Microsoft Message Queuing service is running
        /// </summary>
        /// <returns><c>true</c> if the service is running; otherwise <c>false</c></returns>
        public static bool isMSMQServiceRunning()
        {
            var lockTaken = false;
            if (!Factory.IsWebApp)
            {
                Monitor.Enter(_lock, ref lockTaken);
                try
                {
                    using (var sc = new ServiceController("Message Queuing"))
                    {
                        return sc.Status == ServiceControllerStatus.Running;
                    }
                }
                finally
                {
                    if (lockTaken) { Monitor.Exit(_lock); }
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Wait for MSMQ to start if it is not already running
        /// </summary>
        /// <param name="timeout"><code>TimeSpan</code> containing maximum amount of time to wait</param>
        /// <returns><code>bool</code> indicating if MSMQ is running</returns>
        public static bool waitForMSMQStartup(TimeSpan timeout)
        {
            var lockTaken = false;
            if (!Factory.IsWebApp)
            {
                Monitor.Enter(_lock, ref lockTaken);
                try
                {
                    using (var sc = new ServiceController("Microsoft Messaging"))
                    {
                        if (sc.Status != ServiceControllerStatus.Running)
                        {
                            sc.WaitForStatus(ServiceControllerStatus.Running, timeout);
                            switch (sc.Status)
                            {
                                case ServiceControllerStatus.Running:
                                    return true;
                                default:
                                    return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                finally
                {
                    if (lockTaken) { Monitor.Exit(_lock); }
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Determine if queue exists - this method does NOT auto-create queue if it does not exist
        /// </summary>
        /// <param name="queuePath">Queue path/name</param>
        /// <returns><code>bool</code> indicating existence of queue</returns>
        public static bool QueueExists(string queuePath)
        {
            return MessageQueue.Exists(queuePath);
        }

        /// <summary>
        /// Write message to queue
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="correlationID">Correlation Id - <code>null</code> if none</param>
        /// <param name="queuePath">Path/Name of queue</param>
        public static void queueMessage(string message, string correlationID, string queuePath)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                if (string.IsNullOrEmpty(queuePath))
                {
                    throw new MSMQException("MSMQ::queueMessage Queue name missing.");
                }
                if (!QueueExists(queuePath))
                {
                    throw new MSMQException("MSMQ::queueMessage Queue does not exist.");
                }
                using (var queue = new MessageQueue(queuePath))
                {
                    queue.Formatter = new BinaryMessageFormatter();
                    var Buffer = (byte[])Array.CreateInstance(typeof(byte), _encoder.GetByteCount(message));
                    Buffer = _encoder.GetBytes(message);
                    using (var MSMQ_Message = new Message(Buffer, new BinaryMessageFormatter()))
                    {
                        if (!string.IsNullOrEmpty(correlationID)) { MSMQ_Message.CorrelationId = correlationID; }
                        queue.Send(MSMQ_Message);
                    }
                }
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Write message to queue
        /// </summary>
        /// <param name="message">Message to write (object must implement ISerializable)</param>
        /// <param name="correlationID">Correlation Id - <code>null</code> if none</param>
        /// <param name="queuePath">Path/Name of queue</param>
        public static void queueMessage(object message, string correlationID, string queuePath)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                if(!(message is System.Runtime.Serialization.ISerializable))
                {
                    throw new MSMQException("MSMQ::queueMessage Message object must implement ISerializable.");
                }
                if (string.IsNullOrEmpty(queuePath))
                {
                    throw new MSMQException("MSMQ::queueMessage Queue name missing.");
                }
                if (!QueueExists(queuePath))
                {
                    throw new MSMQException("MSMQ::queueMessage Queue does not exist.");
                }
                using (var queue = new MessageQueue(queuePath))
                {
                    queue.Formatter = new BinaryMessageFormatter();
                    using (var MSMQ_Message = new Message(message))
                    {
                        if (!string.IsNullOrEmpty(correlationID)) { MSMQ_Message.CorrelationId = correlationID; }
                        queue.Send(MSMQ_Message);
                    }
                }
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Read message from queue - message is removed from queue
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="queuePath">Path/Name of queue</param>
        /// <returns><code>object</code> of type T - <code>default(T)</code> if there are no pending messages in the queue</returns>
        public static T readQueue<T>(string queuePath)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                if (string.IsNullOrEmpty(queuePath))
                {
                    throw new MSMQException("MSMQ::readQueue Queue name missing.");
                }
                if (!QueueExists(queuePath))
                {
                    throw new MSMQException("MSMQ::readQueue Queue does not exist.");
                }
                using (var queue = new MessageQueue(queuePath))
                {
                    queue.Formatter = new BinaryMessageFormatter();
                    var message = new Message();
                    queue.MessageReadPropertyFilter.SetAll();
                    try
                    {
                        message = queue.Receive(new TimeSpan(0, 0, 0));
                        if (typeof(T).ToString() == "System.String")
                        {
                            object returnObj = _encoder.GetString((byte[])message.Body);
                            return (T)returnObj;
                        }
                        return (T)message.Body;
                    }
                    catch (Exception ex)
                    {
                        // no-op
                        var dummy = ex.Message;
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
        /// Read message from queue - message is removed from queue
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="correlationId">Correlation Id - <code>null</code> if none</param>
        /// <param name="queuePath">Path/Name of queue</param>
        /// <returns><code>object</code> of type T - <code>default(T)</code> if there are no pending messages in the queue</returns>
        public static T readQueueById<T>(string correlationId, string queuePath)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                if (string.IsNullOrEmpty(queuePath))
                {
                    throw new MSMQException("MSMQ::readQueue Queue name missing.");
                }
                if (!QueueExists(queuePath))
                {
                    throw new MSMQException("MSMQ::readQueue Queue does not exist.");
                }
                using (var queue = new MessageQueue(queuePath))
                {
                    queue.Formatter = new BinaryMessageFormatter();
                    var message = new Message();
                    queue.MessageReadPropertyFilter.SetAll();
                    try
                    {
                        message = queue.ReceiveByCorrelationId(correlationId, new TimeSpan(0, 0, 0));
                        if (typeof(T).ToString() == "System.String")
                        {
                            object returnObj = _encoder.GetString((byte[])message.Body);
                            return (T)returnObj;
                        }
                        return (T)message.Body;
                    }
                    catch (Exception ex)
                    {
                        // no-op
                        var dummy = ex.Message;
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
        /// Read next message from queue - message is NOT removed from queue
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="queuePath">Path/Name of queue</param>
        /// <returns><code>object</code> - <code>null</code> if there are no pending messages in the queue</returns>
        public static T peekQueue<T>(string queuePath)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                if (string.IsNullOrEmpty(queuePath))
                {
                    throw new MSMQException("MSMQ::peekQueue Queue name missing.");
                }
                if (!QueueExists(queuePath))
                {
                    throw new MSMQException("MSMQ::peekQueue Queue does not exist.");
                }
                using (var queue = new MessageQueue(queuePath))
                {
                    queue.Formatter = new BinaryMessageFormatter();
                    var message = new Message();
                    queue.MessageReadPropertyFilter.SetAll();
                    try
                    {
                        message = queue.Peek(new TimeSpan(0, 0, 1));
                        if (typeof(T).ToString() == "System.String")
                        {
                            object returnObj = _encoder.GetString((byte[])message.Body);
                            return (T)returnObj;
                        }
                        return (T)message.Body;
                    }
                    catch (Exception ex)
                    {
                        // no-op
                        var dummy = ex.Message;
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
        /// Determine number of messages currently in queue
        /// </summary>
        /// <param name="queuePath">Path/Name of queue</param>
        /// <returns><code>UInt32</code> number of messages pending in queue</returns>
        public static uint queueDepth(string queuePath)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                if (!QueueExists(queuePath))
                {
                    return 0;
                }

                var props = new NativeMethods.MQMGMTPROPS { cProp = 1 };
                try
                {
                    props.aPropID = Marshal.AllocHGlobal(sizeof(int));
                    Marshal.WriteInt32(props.aPropID, NativeMethods.PROPID_MGMT_QUEUE_MESSAGE_COUNT);

                    props.aPropVar = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.MQPROPVariant)));
                    Marshal.StructureToPtr(new NativeMethods.MQPROPVariant { vt = NativeMethods.VT_NULL }, props.aPropVar, false);

                    props.status = Marshal.AllocHGlobal(sizeof(int));
                    Marshal.WriteInt32(props.status, 0);

                    var result = NativeMethods.MQMgmtGetInfo(null, "queue=Direct=OS:" + queuePath, ref props);
                    if (result != 0 || Marshal.ReadInt32(props.status) != 0)
                    {
                        return 0;
                    }

                    var propVar = (NativeMethods.MQPROPVariant)Marshal.PtrToStructure(props.aPropVar, typeof(NativeMethods.MQPROPVariant));
                    if (propVar.vt != NativeMethods.VT_UI4)
                    {
                        return 0;
                    }
                    else
                    {
                        return propVar.ulVal;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(props.aPropID);
                    Marshal.FreeHGlobal(props.aPropVar);
                    Marshal.FreeHGlobal(props.status);
                }
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }
    }
}

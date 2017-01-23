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

using Ionic.Zip;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Minimal.Common;

namespace Minimal.Utility
{
    /// <summary>
    /// This class maintains application log files
    /// To use this class, the account under which your
    /// application runs must have permission to write
    /// to the application's folder. Alternatively, a
    /// log folder can be specified in the application's
    /// .config file in the appSettings section.
    /// Example:
    /// &lt;appSettings&gt;
    ///     &lt;add key="ApplicationLogFolder" value="c:\logs\" /&gt;
    /// &lt;/appSettings&gt;
    /// </summary>
    /// <remarks>
    /// Required 3rd Party Assemblies:
    /// Ionic.Zip.dll (DotNetZipLib)
    /// 
    /// Internal class dependencies:
    /// Minimal.Utility.FileIO
    /// Minimal.Utility.ExceptionParser
    /// </remarks>
    public static class LogEngine
    {
        private static readonly object _lock = new object();
        private static bool _initialized = false;
        private static string _assemblyName = string.Empty;
        private static string _callingClassName = string.Empty;
        private static string _callingMethodName = string.Empty;
        private static string _callingMethodSignature = string.Empty;
        private static Encoding _encoder;
        private static string _newline = Environment.NewLine;

        /// <summary>
        /// Retrieve current log file name
        /// </summary>
        public static string CurrentLogFileName
        {
            get
            {
                Init();
                return GenerateFileName();
            }
        }

        /// <summary>
        /// Gets archive file name based on date
        /// </summary>
        /// <param name="date"><code>DateTime</code> used to generate archive file name</param>
        /// <returns><code>string</code> containing file name</returns>
        public static string ArchiveFileName(DateTime date)
        {
            Init();
            return GenerateArchiveFileName(date);
        }

        /// <summary>
        /// Retrieve application log folder
        /// </summary>
        public static string ApplicationLogFolder
        {
            get
            {
                Init();
                return GeneratePath();
            }
        }

        /// <summary>
        /// Write a message to the current log file
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="verbose"><code>bool</code> flag indicating if logging non-exception messages</param>
        public static void Log(string message, bool verbose)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                if (verbose)
                {
                    Init();
                    Cleanup();
                    var sf = new StackTrace().GetFrame(1);
                    _callingMethodName = sf.GetMethod().Name;
                    _callingClassName = sf.GetMethod().DeclaringType.Name;
                    _callingMethodSignature = GetMethodSignature(sf);
                    WriteToFile(GenerateEntryHeader() + message + _newline + _newline);
                }
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Write a message to the current log file
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="verbose"><code>bool</code> flag indicating if logging non-exception messages</param>
        /// <param name="encoder"><code>Encoding</code> to use when writing entries to log file</param>
        public static void Log(string message, bool verbose, Encoding encoder)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                if (verbose)
                {
                    Init(encoder);
                    Cleanup();
                    var sf = new StackTrace().GetFrame(1);
                    _callingMethodName = sf.GetMethod().Name;
                    _callingClassName = sf.GetMethod().DeclaringType.Name;
                    _callingMethodSignature = GetMethodSignature(sf);
                    WriteToFile(GenerateEntryHeader() + message + _newline + _newline);
                }
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Write an exception message to the current log file
        /// </summary>
        /// <param name="ex"><code>Exception</code> to parse and log</param>
        public static void Log(Exception ex)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                Init();
                Cleanup();
                var sf = new StackTrace().GetFrame(1);
                _callingMethodName = sf.GetMethod().Name;
                _callingClassName = sf.GetMethod().DeclaringType.Name;
                _callingMethodSignature = GetMethodSignature(sf);
                WriteToFile(GenerateEntryHeader() + ParseException(ex) + _newline + _newline);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Write an exception message to the current log file
        /// </summary>
        /// <param name="ex"><code>Exception</code> to parse and log</param>
        /// <param name="encoder"><code>Encoding</code> to use when writing entries to log file</param>
        public static void Log(Exception ex, Encoding encoder)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                Init(encoder);
                Cleanup();
                var sf = new StackTrace().GetFrame(1);
                _callingMethodName = sf.GetMethod().Name;
                _callingClassName = sf.GetMethod().DeclaringType.Name;
                _callingMethodSignature = GetMethodSignature(sf);
                WriteToFile(GenerateEntryHeader() + ParseException(ex) + _newline + _newline);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Initialize class
        /// </summary>
        private static void Init()
        {
            Init(Factory.GetStandardEncoder);
        }

        /// <summary>
        /// Initialize class
        /// </summary>
        /// <param name="encoder"><code>Encoding</code> to use when writing entries to log file</param>
        private static void Init(Encoding encoder)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                _encoder = encoder;
                if (!_initialized)
                {
                    AssemblyInfo info;
                    if (Assembly.GetEntryAssembly() != null)
                    {
                        info = new AssemblyInfo(Assembly.GetEntryAssembly());
                    }
                    else
                    {
                        info = new AssemblyInfo(Assembly.GetCallingAssembly());
                    }
                    _assemblyName = info.AssemblyName;
                }
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Get signature of calling method
        /// </summary>
        /// <param name="sf"><code>StackFrame</code></param>
        /// <returns><code>string</code> containing method signature</returns>
        private static string GetMethodSignature(StackFrame sf)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                Init();
                var pi = sf.GetMethod().GetParameters();
                if (pi.GetLength(0) > 0)
                {
                    var sb = new StringBuilder();
                    sb.Append("(");
                    foreach (var info in pi)
                    {
                        if (info.ParameterType.ToString().Substring(0, 6).ToUpper() == "SYSTEM")
                        {
                            sb.Append(info.ParameterType.ToString().Remove(0, info.ParameterType.ToString().LastIndexOf('.') + 1));
                        }
                        else
                        {
                            sb.Append(info.ParameterType.ToString());
                        }
                        sb.Append(" ");
                        sb.Append(info.Name);
                        sb.Append(", ");
                    }
                    sb.Append(")");
                    return sb.ToString().Remove(sb.ToString().Length - 3, 2);
                }
                else
                {
                    return string.Empty;
                }
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Parse <code>Exception</code> to formatted string
        /// </summary>
        /// <param name="ex"><code>Exception</code> to parse</param>
        /// <returns><code>string</code> containing parser formatted <code>Exception></code></returns>
        private static string ParseException(Exception ex)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                Init();
                return ExceptionParser.Parse(ex);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Write to file
        /// </summary>
        /// <param name="message">Message to write</param>
        private static void WriteToFile(string message)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                Init();
                FileIO.WriteTextFile(GenerateFileSpec(), FileMode.Append, message);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Generate the path to the application log folder
        /// </summary>
        /// <returns><code>string</code> containing path</returns>
        private static string GeneratePath()
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                Init();
                var returnValue = new StringBuilder();
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ApplicationLogFolder"]))
                {
                    returnValue.Append(ConfigurationManager.AppSettings["ApplicationLogFolder"]);
                }
                else
                {
                    if (Assembly.GetEntryAssembly() != null)
                    {
                        returnValue.Append(Path.GetDirectoryName(Assembly.GetEntryAssembly().CodeBase).Replace("file:\\", string.Empty));
                    }
                    else
                    {
                        returnValue.Append(Path.GetDirectoryName(Assembly.GetCallingAssembly().CodeBase).Replace("file:\\", string.Empty));
                    }
                    returnValue.Append("\\");
                }
                returnValue.Append(_assemblyName);
                returnValue.Append(".logs\\");
                if (!Directory.Exists(returnValue.ToString())) { Directory.CreateDirectory(returnValue.ToString()); }
                return returnValue.ToString();
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Generate log file name
        /// </summary>
        /// <returns><code>string</code> containing file name</returns>
        private static string GenerateFileName()
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                Init();
                var returnValue = new StringBuilder();
                returnValue.Append(_assemblyName);
                returnValue.Append(".");
                returnValue.Append(DateTime.Now.ToString("MM-dd-yyyy"));
                returnValue.Append(".log");
                return returnValue.ToString();
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Generate file specification (path\filename)
        /// </summary>
        /// <returns><code>string</code> containing file specification</returns>
        private static string GenerateFileSpec()
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                Init();
                var returnValue = new StringBuilder();
                returnValue.Append(GeneratePath());
                if (!Directory.Exists(returnValue.ToString())) { Directory.CreateDirectory(returnValue.ToString()); }
                returnValue.Append(GenerateFileName());
                return returnValue.ToString();
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Generate archive file name
        /// </summary>
        /// <param name="date"><code>DateTime</code> on which to base file name</param>
        /// <returns><code>string</code> containing file name</returns>
        private static string GenerateArchiveFileName(DateTime date)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                Init();
                var returnValue = new StringBuilder();
                returnValue.Append(_assemblyName);
                returnValue.Append(".");
                returnValue.Append(date.ToString("MM-yyyy"));
                returnValue.Append(".log.Archive.zip");
                return returnValue.ToString();
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Generate archive file name based on log file name
        /// </summary>
        /// <param name="fileName">Log file name used to generate archive file name</param>
        /// <returns><code>string</code> containing file name</returns>
        private static string GenerateArchiveFileName(string fileName)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                Init();
                var returnValue = new StringBuilder();
                returnValue.Append(_assemblyName);
                returnValue.Append(".");
                fileName = fileName.Replace(".log", string.Empty);
                var dateString = fileName.Substring(fileName.Length - 10, 10);
                var dateParts = dateString.Split('-');
                returnValue.Append(dateParts[0]);
                returnValue.Append("-");
                returnValue.Append(dateParts[2]);
                returnValue.Append(".log.Archive.zip");
                return returnValue.ToString();
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Generate archive file specification (path\filename) based on log file name
        /// </summary>
        /// <param name="fileName">Log file name on which to base archive file specification</param>
        /// <returns><code>string</code> containing file specification</returns>
        private static string GenerateArchiveFileSpec(string fileName)
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                Init();
                var returnValue = new StringBuilder();
                returnValue.Append(GeneratePath());
                returnValue.Append(GenerateArchiveFileName(fileName));
                return returnValue.ToString();
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Examines all files in application log folder and archives appropriate files
        /// runs each time a new message is logged
        /// </summary>
        private static void Cleanup()
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                Init();
                var files = Directory.GetFiles(GeneratePath());
                foreach (var file in files)
                {
                    if (file != GenerateFileSpec())
                    {
                        var info = new FileInfo(file);
                        if (info.Extension != ".zip")
                        {
                            var archiveFile = GenerateArchiveFileSpec(file);
                            var zFile = new ZipFile();
                            if (!File.Exists(archiveFile))
                            {
                                zFile = new ZipFile(archiveFile);
                            }
                            else
                            {
                                zFile = ZipFile.Read(archiveFile);
                            }
                            zFile.AddFile(file, string.Empty);
                            zFile.Save(archiveFile);
                            zFile.Dispose();
                            FileIO.Delete(file, false);
                        }
                    }
                }
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Generate log entry heaeder
        /// </summary>
        /// <returns><code>string</code> containing log entry header</returns>
        private static string GenerateEntryHeader()
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                Init();
                var sb = new StringBuilder();
                sb.Append(DateTime.Now.ToString("[MM/dd/yyyy HH:mm:ss.fff] "));
                sb.Append(_assemblyName);
                sb.Append(".");
                sb.Append(_callingClassName);
                sb.Append("::");
                sb.Append(_callingMethodName);
                sb.Append(_callingMethodSignature);
                sb.Append(_newline);
                return sb.ToString();
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }
    }
}

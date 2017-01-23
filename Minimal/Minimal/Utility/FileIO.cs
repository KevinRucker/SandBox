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
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Minimal.Utility
{
	/// <summary>
	/// File Input/Output
	/// </summary>
	public static class FileIO
	{
		// Locking object
		private static readonly object _lock = new object();
		// Standard byte encoding - UTF8
		private static Encoding _standardEncoding = Factory.GetStandardEncoder;

		// Lambda function definitions
		private static Func<string, FileStream> OpenFileRead = x => new FileStream(x, FileMode.Open, FileAccess.Read);
		private static Func<string, FileMode, FileStream> OpenFileWrite = (x, y) => new FileStream(x, y, FileAccess.Write);

		/// <summary>
		/// Read a binary file
		/// </summary>
		/// <param name="FileSpec">File to read</param>
		/// <returns><c>byte[]</c> containing the contents of the file</returns>
		public static byte[] ReadBinaryFile(string FileSpec)
		{
			var locked = false;
			try
			{
				Monitor.Enter(_lock, ref locked);
				using (var hFile = OpenFileRead(FileSpec))
				{
					return StreamUtility.ReadFullStream(hFile);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("FileIO::readBinaryFile File IO Exception", ex);
			}
			finally
			{
				if (locked) { Monitor.Exit(_lock); }
			}
		}

		/// <summary>
		/// Read a text file
		/// </summary>
		/// <param name="FileSpec">File to read</param>
		/// <returns><c>string</c> containing the contents of the file</returns>
		public static string ReadTextFile(string FileSpec)
		{
			return ReadTextFile(FileSpec, _standardEncoding);
		}

		/// <summary>
		/// Read a text file
		/// </summary>
		/// <param name="FileSpec">File to read</param>
		/// <param name="FileEncoding">The text encoding to use</param>
		/// <returns><c>string</c> containing the contents of the file</returns>
		public static string ReadTextFile(string FileSpec, Encoding FileEncoding)
		{
			var locked = false;
			try
			{
				Monitor.Enter(_lock, ref locked);
				return Factory.EncodeBytes(FileEncoding, ReadBinaryFile(FileSpec));
			}
			catch (Exception ex)
			{
				throw new Exception("FileIO::readTextFile File IO Exception", ex);
			}
			finally
			{
				if (locked) { Monitor.Exit(_lock); }
			}
		}

		/// <summary>
		/// Write a binary file
		/// </summary>
		/// <param name="FileSpec">File to write</param>
		/// <param name="Mode"><see cref="System.IO.FileMode"/></param>
		/// <param name="content"><c>byte[]</c> to write to the file</param>
		public static void WriteBinaryFile(string FileSpec, FileMode Mode, byte[] content)
		{
			var locked = false;
			try
			{
				Monitor.Enter(_lock, ref locked);
				using (var hFile = OpenFileWrite(FileSpec, Mode))
				{
					hFile.Write(content, 0, content.Length);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("FileIO::writeBinaryFile File IO Exception", ex);
			}
			finally
			{
				if (locked) { Monitor.Exit(_lock); }
			}
		}

		/// <summary>
		/// Write a text file
		/// </summary>
		/// <param name="FileSpec">File to write</param>
		/// <param name="Mode"><see cref="System.IO.FileMode"/></param>
		/// <param name="Content"><c>string</c> to write to the file</param>
		public static void WriteTextFile(string FileSpec, FileMode Mode, string Content)
		{
			WriteTextFile(FileSpec, Mode, Content, _standardEncoding);
		}

		/// <summary>
		/// Write a text file
		/// </summary>
		/// <param name="FileSpec">File to write</param>
		/// <param name="Mode"><see cref="System.IO.FileMode"/></param>
		/// <param name="Content"><c>string</c> to write to the file</param>
		/// <param name="FileEncoding">The text encoding to use</param>
		public static void WriteTextFile(string FileSpec, FileMode Mode, string Content, Encoding FileEncoding)
		{
			var locked = false;
			try
			{
				Monitor.Enter(_lock, ref locked);
				WriteBinaryFile(FileSpec, Mode, Factory.DecodeString(FileEncoding, Content));
			}
			catch (Exception ex)
			{
				throw new Exception("FileIO::writeTextFile File IO Exception", ex);
			}
			finally
			{
				if (locked) { Monitor.Exit(_lock); }
			}
		}

		/// <summary>
		/// Deletes the file
		/// </summary>
		/// <param name="fileSpec">The file to delete</param>
		/// <param name="secure"><c>true</c> if using secure delete; otherwise <c>false</c></param>
		public static void Delete(string fileSpec, bool secure)
		{
			var locked = false;
			try
			{
				Monitor.Enter(_lock, ref locked);
				if (File.Exists(fileSpec))
				{
					if (secure)
					{
						// Secure Delete - Follows DoD standard outlined in DoD Manual 5220.22 M
						// This option can be relatively slow, depending on file size, but causes the file to be unrecoverable by conventional means
						try
						{
							using (var hFile = OpenFileWrite(fileSpec, FileMode.Open))
							{
								// Overwrite with 0s
								for (var i = 0; i < hFile.Length; i++)
								{
									hFile.WriteByte(0);
								}
								hFile.Flush();
								hFile.Seek(0, SeekOrigin.Begin);

								// Overwrite with 1s
								for (var i = 0; i < hFile.Length; i++)
								{
									hFile.WriteByte(1);
								}
								hFile.Flush();
								hFile.Seek(0, SeekOrigin.Begin);

								// Overwrite with pseudo-random bytes
								var rng = new RNGCryptoServiceProvider();
								for (long i = 0; i < hFile.Length; i++)
								{
									var random = new byte[1];
									rng.GetBytes(random);
									hFile.WriteByte(random[0]);
								}
								hFile.Flush();
								rng = null;
							}
						}
						catch (Exception ex)
						{
							throw new Exception("FileIO::delete Exception (secure delete)", ex);
						}
					}

					// Delete the file
					try
					{
						File.Delete(fileSpec);
					}
					catch (Exception ex)
					{
						throw new Exception("FileIO::delete Exception", ex);
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

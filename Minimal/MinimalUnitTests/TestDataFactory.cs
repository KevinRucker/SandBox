using Minimal.Common;
using Minimal.Data;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace MinimalUnitTests
{
    public static class TestDataFactory
    {
        private static readonly object _lock = new object(); // Locking construct
        private const string _datafilename = "MinimalUnitTests.lorem_ipsum.txt";
        private const string _foxtest = "The quick brown fox jumped over the lazy dog's back. 1234567890";
        private const string _fillertext = "Now is the time for all good men to come to the aid of their country. 1234567890";

        public static Func<string, bool, string> GetDBConnectionString = (x, y) => DBConfigFileCSProvider.Factory(x, y).GetConnectionString();
        public static Func<string> GetFoxTest = () => _foxtest;
        public static Func<string> GetFillerText = () => _fillertext;
        public static Func<Uri> GetGoogle = () => new Uri("http://www.google.com");

        public static string GetLoremIpsumString(int length, Encoding encoder)
        {
            bool _locked = false;
            try
            {
                Monitor.Enter(_lock, ref _locked);
                using (Stream stream = GetLoremIpsumStream())
                {
                    if (length > stream.Length)
                    {
                        throw new Exception("TestDataFactory::GetLoremIpsumString Requested length exceeds the length of the available test data [" + stream.Length.ToString() + " bytes].");
                    }
                    var buffer = Factory.CreateBuffer(length);
                    stream.Read(buffer, 0, length);
                    return encoder.GetString(buffer);
                }
            }
            finally
            {
                if (_locked)
                {
                    Monitor.Exit(_lock);
                }
            }
        }

        public static byte[] GetLoremIpsumBytes(int length)
        {
            bool _locked = false;
            try
            {
                Monitor.Enter(_lock, ref _locked);
                using (Stream stream = GetLoremIpsumStream())
                {
                    if (length > stream.Length)
                    {
                        throw new Exception("TestDataFactory::GetLoremIpsumBytes Requested length exceeds the length of the available test data [" + stream.Length.ToString() + " bytes].");
                    }
                    var buffer = Factory.CreateBuffer(length);
                    stream.Read(buffer, 0, length);
                    return buffer;
                }
            }
            finally
            {
                if (_locked)
                {
                    Monitor.Exit(_lock);
                }
            }
        }

        public static Stream GetLoremIpsumStream()
        {
            bool _locked = false;
            try
            {
                Monitor.Enter(_lock, ref _locked);
                return GetEmbeddedResource(_datafilename);
            }
            finally
            {
                if (_locked)
                {
                    Monitor.Exit(_lock);
                }
            }
        }

        private static Stream GetEmbeddedResource(string name)
        {
            bool _locked = false;
            try
            {
                Monitor.Enter(_lock, ref _locked);
                var asm = Assembly.GetExecutingAssembly();
                return asm.GetManifestResourceStream(name);
            }
            finally
            {
                if (_locked)
                {
                    Monitor.Exit(_lock);
                }
            }
        }
    }
}

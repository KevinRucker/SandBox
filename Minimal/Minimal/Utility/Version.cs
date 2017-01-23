using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic.ApplicationServices;

namespace Minimal.Utility
{
    /// <summary>
    /// Static class providing methods to query the library and application versions
    /// </summary>
    public static class Version
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// Returns the version of this library
        /// </summary>
        /// <returns><c>String</c> containing the library version</returns>
        public static string LibraryVersion()
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                var sb = new StringBuilder();
                var aInfo = new AssemblyInfo(Assembly.GetAssembly(typeof(Version)));
                sb.Append(aInfo.Version.Major.ToString());
                sb.Append(".");
                sb.Append(aInfo.Version.Minor.ToString());
                sb.Append(".");
                sb.Append(aInfo.Version.Build.ToString());
                if (aInfo.Version.Revision != 0)
                {
                    sb.Append(".");
                    sb.Append(aInfo.Version.Revision.ToString());
                }
                return sb.ToString();
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Returns the version of the application calling this library
        /// </summary>
        /// <returns><c>String</c> containing the application version</returns>
        public static string ApplicationVersion()
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                var sb = new StringBuilder();
                AssemblyInfo aInfo;
                if (Assembly.GetEntryAssembly() != null)
                {
                    aInfo = new AssemblyInfo(Assembly.GetEntryAssembly());
                }
                else
                {
                    aInfo = new AssemblyInfo(Assembly.GetCallingAssembly());
                }
                sb.Append(aInfo.Version.Major.ToString());
                sb.Append(".");
                sb.Append(aInfo.Version.Minor.ToString());
                sb.Append(".");
                sb.Append(aInfo.Version.Build.ToString());
                if (aInfo.Version.Revision != 0)
                {
                    sb.Append(".");
                    sb.Append(aInfo.Version.Revision.ToString());
                }
                return sb.ToString();
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }
    }
}

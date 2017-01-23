using Minimal.Common;
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Minimal.Security
{
    /// <summary>
    /// Determine if the current process is running with elevated permissions
    /// </summary>
    public class Process
    {
        /// <summary>
        /// Determine if the current process is running with elevated permissions
        /// </summary>
        /// <returns><c>true</c> if running with elevated permissions; otherwise <c>false</c></returns>
        public static bool IsRunningElevated()
        {
            var identity = WindowsIdentity.GetCurrent();
            if (identity == null) throw new InvalidOperationException("Couldn't get the current user identity");

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
                        // TokenElevationTypeFull - User has a split token, and the process is running elevated.
                        return true;
                    case NativeMethods.TokenElevationType.TokenElevationTypeLimited:
                        // TokenElevationTypeLimited - User has a split token, but the process is not running elevated.
                        return false;
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
    }
}

using System;
using System.Runtime.InteropServices;

namespace Lunatics.SDLGL.Utilities
{
	public static class OSVersionExt
	{

        [DllImport("libc")]
        private static extern int uname(IntPtr buffer);

		public static bool IsWindows(this OperatingSystem @this)
		{
			var platform = @this.Platform;
			return platform == PlatformID.Win32NT ||
			       platform == PlatformID.Win32S ||
			       platform == PlatformID.Win32Windows ||
			       platform == PlatformID.WinCE;
		}

		public static bool IsMacOSX(this OperatingSystem @this)
		{
            if (@this.Platform == PlatformID.Unix)
            {
                var buffer = IntPtr.Zero;
                try
                {
                    buffer = Marshal.AllocHGlobal(8192);
                    if (uname(buffer) == 0)
                    {
                        var os = Marshal.PtrToStringAnsi(buffer);
                        if (os == "Darwin")
                            return true;
                    }
                }
                catch { }
                finally
                {
                    if (buffer != IntPtr.Zero)
                        Marshal.FreeHGlobal(buffer);
                }
            }

            return @this.Platform == PlatformID.MacOSX;
		}

        private static void Initialize()
        {
            if (_initialized) return;
        }

        #region Fields

        private static bool _isWindows = false;
        private static bool _isLunux = false;
        private static bool _isMacOSX = false;
        private static bool _initialized = false;

        #endregion
    }
}
using System;

namespace Lunatics.Utilities
{
	public static class OSVersionExt
	{
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
			return @this.Platform == PlatformID.MacOSX;
		}
	}
}
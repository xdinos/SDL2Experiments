using System;
using System.Runtime.InteropServices;
using Lunatics.Utilities;

namespace Lunatics.Framework.DesktopGL.Utilities
{
	internal class FuncLoader
	{
		private const int RTLD_LAZY = 0x0001;

		public static IntPtr LoadLibrary(string libname)
		{
			if (Environment.OSVersion.IsWindows())
				return Windows.LoadLibraryW(libname);

			if (Environment.OSVersion.IsMacOSX())
				return OSX.dlopen(libname, RTLD_LAZY);

			return Linux.dlopen(libname, RTLD_LAZY);
		}

		public static T LoadFunction<T>(IntPtr library, string function, bool throwIfNotFound = false)
		{
			IntPtr result;

			if (Environment.OSVersion.IsWindows())
				result = Windows.GetProcAddress(library, function);
			else if (Environment.OSVersion.IsMacOSX())
				result = OSX.dlsym(library, function);
			else
				result = Linux.dlsym(library, function);

			if (result == IntPtr.Zero)
			{
				if (throwIfNotFound)
					throw new EntryPointNotFoundException(function);

				return default(T);
			}

			return Marshal.GetDelegateForFunctionPointer<T>(result);
		}

		private class Windows
		{
			[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
			public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

			[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
			public static extern IntPtr LoadLibraryW(string lpszLib);
		}

		private class Linux
		{
			[DllImport("libdl.so.2")]
			public static extern IntPtr dlopen(string path, int flags);

			[DllImport("libdl.so.2")]
			public static extern IntPtr dlsym(IntPtr handle, string symbol);
		}

		private class OSX
		{
			[DllImport("/usr/lib/libSystem.dylib")]
			public static extern IntPtr dlopen(string path, int flags);

			[DllImport("/usr/lib/libSystem.dylib")]
			public static extern IntPtr dlsym(IntPtr handle, string symbol);
		}
	}
}
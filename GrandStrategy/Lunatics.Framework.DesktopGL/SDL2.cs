using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Lunatics.Framework.DesktopGL.Utilities;
using Lunatics.Utilities;

namespace Lunatics.Framework.DesktopGL
{
	internal static class Sdl
	{
		[Flags]
		public enum InitFlags : int
		{
			Video = 0x00000020,
			//Joystick = 0x00000200,
			//Haptic = 0x00001000,
			//GameController = 0x00002000,
		}

		public struct Rectangle
		{
			public int X;
			public int Y;
			public int Width;
			public int Height;
		}

		public struct Version
		{
			public byte Major;
			public byte Minor;
			public byte Patch;
		}

		public enum EventType : uint
		{
			First = 0,

			Quit = 0x100,

			WindowEvent = 0x200,
			SysWM = 0x201,

			KeyDown = 0x300,
			KeyUp = 0x301,
			TextEditing = 0x302,
			TextInput = 0x303,

			MouseMotion = 0x400,
			MouseButtonDown = 0x401,
			MouseButtonup = 0x402,
			MouseWheel = 0x403,

			JoyAxisMotion = 0x600,
			JoyBallMotion = 0x601,
			JoyHatMotion = 0x602,
			JoyButtonDown = 0x603,
			JoyButtonUp = 0x604,
			JoyDeviceAdded = 0x605,
			JoyDeviceRemoved = 0x606,

			ControllerAxisMotion = 0x650,
			ControllerButtonDown = 0x651,
			ControllerButtonUp = 0x652,
			ControllerDeviceAdded = 0x653,
			ControllerDeviceRemoved = 0x654,
			ControllerDeviceRemapped = 0x654,

			FingerDown = 0x700,
			FingerUp = 0x701,
			FingerMotion = 0x702,

			DollarGesture = 0x800,
			DollarRecord = 0x801,
			MultiGesture = 0x802,

			ClipboardUpdate = 0x900,

			DropFile = 0x1000,

			AudioDeviceAdded = 0x1100,
			AudioDeviceRemoved = 0x1101,

			RenderTargetsReset = 0x2000,
			RenderDeviceReset = 0x2001,

			UserEvent = 0x8000,

			Last = 0xFFFF
		}

		[StructLayout(LayoutKind.Explicit, Size = 56)]
		public struct Event
		{
			[FieldOffset(0)]
			public EventType Type;
			[FieldOffset(0)]
			public Window.Event Window;
			[FieldOffset(0)]
			public Keyboard.Event Key;
			[FieldOffset(0)]
			public Mouse.MotionEvent Motion;
			[FieldOffset(0)]
			public Mouse.ButtonEvent Button;
			[FieldOffset(0)]
			public Mouse.WheelEvent Wheel;
			// TODO: ...
			//[FieldOffset(0)]
			//public Keyboard.TextEditingEvent Edit;
			//[FieldOffset(0)]
			//public Keyboard.TextInputEvent Text;
			//[FieldOffset(0)]
			//public Joystick.DeviceEvent JoystickDevice;
			//[FieldOffset(0)]
			//public GameController.DeviceEvent ControllerDevice;
		}

		private static IntPtr NativeLibrary { get; } = GetNativeLibrary();

		public static Version GetVersion()
		{
			GetVersionNative(out var version);
			return version;
		}

		public static void Init(InitFlags flags)
		{
			LogOnError(InitNative((int)flags));
		}

		public static string GetPlatform()
		{
			return InteropHelpers.Utf8ToString(ThrowOnError(GetPlatformFunc()));
		}

		public static string GetError()
		{
			return InteropHelpers.Utf8ToString(GetErrorFunc());
		}

		public static int LogOnError(int value)
		{
			if (value<0)
				Debug.WriteLine(GetError());

			return value;
		}
		
		public static IntPtr LogOnError(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
				Debug.WriteLine(GetError());

			return ptr;
		}

		public static int ThrowOnError(int value)
		{
			if (value < 0)
				throw new Exception(GetError());

			return value;
		}

		public static IntPtr ThrowOnError(IntPtr ptr)
		{
			if (ptr == IntPtr.Zero)
				throw new Exception(GetError());

			return ptr;
		}

		#region Delegates

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void d_sdl_clearerror();
		public static d_sdl_clearerror ClearError = FuncLoader.LoadFunction<d_sdl_clearerror>(NativeLibrary, "SDL_ClearError");

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void EnableScreenSaverFunc();
		public static readonly EnableScreenSaverFunc EnableScreenSaver = FuncLoader.LoadFunction<EnableScreenSaverFunc>(NativeLibrary, "SDL_EnableScreenSaver");

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void DisableScreenSaverFunc();
		public static readonly DisableScreenSaverFunc DisableScreenSaver = FuncLoader.LoadFunction<DisableScreenSaverFunc>(NativeLibrary, "SDL_DisableScreenSaver");

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int SetHintFunc(string name, string value);
		public static SetHintFunc SetHint = FuncLoader.LoadFunction<SetHintFunc>(NativeLibrary, "SDL_SetHint");

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int PollEventFunc([Out] out Event _event);
		public static PollEventFunc PollEvent = FuncLoader.LoadFunction<PollEventFunc>(NativeLibrary, "SDL_PollEvent");

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void QuitFunc();
		public static QuitFunc Quit = FuncLoader.LoadFunction<QuitFunc>(NativeLibrary, "SDL_Quit");

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int InitFunc(int flags);
		private static readonly InitFunc InitNative = FuncLoader.LoadFunction<InitFunc>(NativeLibrary, "SDL_Init");

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void SDL_GetVersion(out Version version);
		private static readonly SDL_GetVersion GetVersionNative = FuncLoader.LoadFunction<SDL_GetVersion>(NativeLibrary, nameof(SDL_GetVersion));
		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr SDL_GetPlatform();
		private static SDL_GetPlatform GetPlatformFunc = FuncLoader.LoadFunction<SDL_GetPlatform>(NativeLibrary, nameof(SDL_GetPlatform));

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr SDL_GetError();
		private static SDL_GetError GetErrorFunc = FuncLoader.LoadFunction<SDL_GetError>(NativeLibrary, nameof(SDL_GetError));

		#endregion

		public static class Display
		{
			public struct Mode
			{
				public uint Format;
				public int Width;
				public int Height;
				public int RefreshRate;
				public IntPtr DriverData;
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate int d_sdl_getdisplaybounds(int displayIndex, out Rectangle rect);
			private static d_sdl_getdisplaybounds SDL_GetDisplayBounds = FuncLoader.LoadFunction<d_sdl_getdisplaybounds>(NativeLibrary, "SDL_GetDisplayBounds");

			public static void GetBounds(int displayIndex, out Rectangle rect)
			{
				LogOnError(SDL_GetDisplayBounds(displayIndex, out rect));
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate int d_sdl_getcurrentdisplaymode(int displayIndex, out Mode mode);
			private static d_sdl_getcurrentdisplaymode SDL_GetCurrentDisplayMode = FuncLoader.LoadFunction<d_sdl_getcurrentdisplaymode>(NativeLibrary, "SDL_GetCurrentDisplayMode");

			public static void GetCurrentDisplayMode(int displayIndex, out Mode mode)
			{
				LogOnError(SDL_GetCurrentDisplayMode(displayIndex, out mode));
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate int d_sdl_getdisplaymode(int displayIndex, int modeIndex, out Mode mode);
			private static d_sdl_getdisplaymode SDL_GetDisplayMode = FuncLoader.LoadFunction<d_sdl_getdisplaymode>(NativeLibrary, "SDL_GetDisplayMode");

			public static void GetDisplayMode(int displayIndex, int modeIndex, out Mode mode)
			{
				LogOnError(SDL_GetDisplayMode(displayIndex, modeIndex, out mode));
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate int d_sdl_getclosestdisplaymode(int displayIndex, Mode mode, out Mode closest);
			private static d_sdl_getclosestdisplaymode SDL_GetClosestDisplayMode = FuncLoader.LoadFunction<d_sdl_getclosestdisplaymode>(NativeLibrary, "SDL_GetClosestDisplayMode");

			public static void GetClosestDisplayMode(int displayIndex, Mode mode, out Mode closest)
			{
				LogOnError(SDL_GetClosestDisplayMode(displayIndex, mode, out closest));
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate IntPtr d_sdl_getdisplayname(int index);
			private static d_sdl_getdisplayname SDL_GetDisplayName = FuncLoader.LoadFunction<d_sdl_getdisplayname>(NativeLibrary, "SDL_GetDisplayName");

			public static string GetDisplayName(int index)
			{
				return InteropHelpers.Utf8ToString(LogOnError(SDL_GetDisplayName(index)));
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate int d_sdl_getnumdisplaymodes(int displayIndex);
			private static d_sdl_getnumdisplaymodes SDL_GetNumDisplayModes = FuncLoader.LoadFunction<d_sdl_getnumdisplaymodes>(NativeLibrary, "SDL_GetNumDisplayModes");

			public static int GetNumDisplayModes(int displayIndex)
			{
				return LogOnError(SDL_GetNumDisplayModes(displayIndex));
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate int d_sdl_getnumvideodisplays();
			private static d_sdl_getnumvideodisplays SDL_GetNumVideoDisplays = FuncLoader.LoadFunction<d_sdl_getnumvideodisplays>(NativeLibrary, "SDL_GetNumVideoDisplays");

			public static int GetNumVideoDisplays()
			{
				return LogOnError(SDL_GetNumVideoDisplays());
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate int d_sdl_getwindowdisplayindex(IntPtr window);
			private static d_sdl_getwindowdisplayindex SDL_GetWindowDisplayIndex = FuncLoader.LoadFunction<d_sdl_getwindowdisplayindex>(NativeLibrary, "SDL_GetWindowDisplayIndex");

			public static int GetWindowDisplayIndex(IntPtr window)
			{
				return LogOnError(SDL_GetWindowDisplayIndex(window));
			}
		}

		public static class Window
		{
			public const int PosUndefined = 0x1FFF0000;
			public const int PosCentered = 0x2FFF0000;

			[Flags]
			public enum Flags
			{
				Fullscreen = 0x00000001,
				OpenGL = 0x00000002,
				Shown = 0x00000004,
				Hidden = 0x00000008,
				Borderless = 0x00000010,
				Resizable = 0x00000020,
				Minimized = 0x00000040,
				Maximized = 0x00000080,
				Grabbed = 0x00000100,
				InputFocus = 0x00000200,
				MouseFocus = 0x00000400,
				FullscreenDesktop = 0x00001000 | Fullscreen,
				Foreign = 0x00000800,
				AllowHighDPI = 0x00002000,
				MouseCapture = 0x00004000
			}
			public enum EventId : byte
			{
				None,
				Shown,
				Hidden,
				Exposed,
				Moved,
				Resized,
				SizeChanged,
				Minimized,
				Maximized,
				Restored,
				Enter,
				Leave,
				FocusGained,
				FocusLost,
				Close,
			}

			public static class State
			{
				public const int Fullscreen = 0x00000001;
				public const int OpenGL = 0x00000002;
				public const int Shown = 0x00000004;
				public const int Hidden = 0x00000008;
				public const int Borderless = 0x00000010;
				public const int Resizable = 0x00000020;
				public const int Minimized = 0x00000040;
				public const int Maximized = 0x00000080;
				public const int Grabbed = 0x00000100;
				public const int InputFocus = 0x00000200;
				public const int MouseFocus = 0x00000400;
				public const int FullscreenDesktop = 0x00001001;
				public const int Foreign = 0x00000800;
				public const int AllowHighDPI = 0x00002000;
				public const int MouseCapture = 0x00004000;
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct Event
			{
				public EventType Type;
				public uint TimeStamp;
				public uint WindowID;
				public EventId EventID;
				private byte padding1;
				private byte padding2;
				private byte padding3;
				public int Data1;
				public int Data2;
			}

			public enum SysWMType
			{
				Unknow,
				Windows,
				X11,
				Directfb,
				Cocoa,
				UiKit,
				Wayland,
				Mir,
				WinRt,
				Android
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct SDL_SysWMinfo
			{
				public Version version;
				public SysWMType subsystem;
				public IntPtr window;
			}

			public static IntPtr Create(string title, int x, int y, int w, int h, Flags flags)
			{
				return LogOnError(CreateWindowNative(title, x, y, w, h, (int)flags));
			}

			public static int GetDisplayIndex(IntPtr window)
			{
				return LogOnError(GetWindowDisplayIndex(window));
			}

			public static void SetFullscreen(IntPtr window, int flags)
			{
				LogOnError(SetWindowFullscreen(window, flags));
			}

			public static void SetTitle(IntPtr handle, string title)
			{
				var bytes = System.Text.Encoding.UTF8.GetBytes(title);
				SDL_SetWindowTitle(handle, ref bytes[0]);
			}

			#region Delegates

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void DestroyWindowFunc(IntPtr window);
			public static readonly DestroyWindowFunc Destroy = FuncLoader.LoadFunction<DestroyWindowFunc>(NativeLibrary, "SDL_DestroyWindow");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void ShowWindowFunc(IntPtr window);
			public static ShowWindowFunc Show = FuncLoader.LoadFunction<ShowWindowFunc>(NativeLibrary, "SDL_ShowWindow");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate int d_sdl_getwindowflags(IntPtr window);
			public static d_sdl_getwindowflags GetWindowFlags = FuncLoader.LoadFunction<d_sdl_getwindowflags>(NativeLibrary, "SDL_GetWindowFlags");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void d_sdl_setwindowicon(IntPtr window, IntPtr icon);
			public static d_sdl_setwindowicon SetIcon = FuncLoader.LoadFunction<d_sdl_setwindowicon>(NativeLibrary, "SDL_SetWindowIcon");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void d_sdl_getwindowposition(IntPtr window, out int x, out int y);
			public static d_sdl_getwindowposition GetPosition = FuncLoader.LoadFunction<d_sdl_getwindowposition>(NativeLibrary, "SDL_GetWindowPosition");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void d_sdl_getwindowsize(IntPtr window, out int w, out int h);
			public static d_sdl_getwindowsize GetSize = FuncLoader.LoadFunction<d_sdl_getwindowsize>(NativeLibrary, "SDL_GetWindowSize");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void d_sdl_setwindowbordered(IntPtr window, int bordered);
			public static d_sdl_setwindowbordered SetBordered = FuncLoader.LoadFunction<d_sdl_setwindowbordered>(NativeLibrary, "SDL_SetWindowBordered");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void d_sdl_setwindowposition(IntPtr window, int x, int y);
			public static d_sdl_setwindowposition SetPosition = FuncLoader.LoadFunction<d_sdl_setwindowposition>(NativeLibrary, "SDL_SetWindowPosition");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void d_sdl_setwindowresizable(IntPtr window, bool resizable);
			public static d_sdl_setwindowresizable SetResizable = FuncLoader.LoadFunction<d_sdl_setwindowresizable>(NativeLibrary, "SDL_SetWindowResizable");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void d_sdl_setwindowsize(IntPtr window, int w, int h);
			public static d_sdl_setwindowsize SetSize = FuncLoader.LoadFunction<d_sdl_setwindowsize>(NativeLibrary, "SDL_SetWindowSize");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate void d_sdl_setwindowtitle(IntPtr window, ref byte value);
			private static d_sdl_setwindowtitle SDL_SetWindowTitle = FuncLoader.LoadFunction<d_sdl_setwindowtitle>(NativeLibrary, "SDL_SetWindowTitle");
			
			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate IntPtr CreateWindowFunc(string title, int x, int y, int w, int h, int flags);
			private static readonly CreateWindowFunc CreateWindowNative = FuncLoader.LoadFunction<CreateWindowFunc>(NativeLibrary, "SDL_CreateWindow");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate int GetWindowDisplayIndexFunc(IntPtr window);
			private static readonly GetWindowDisplayIndexFunc GetWindowDisplayIndex = FuncLoader.LoadFunction<GetWindowDisplayIndexFunc>(NativeLibrary, "SDL_GetWindowDisplayIndex");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate int SetWindowFullscreenDelegate(IntPtr window, int flags);
			private static readonly SetWindowFullscreenDelegate SetWindowFullscreen = FuncLoader.LoadFunction<SetWindowFullscreenDelegate>(NativeLibrary, "SDL_SetWindowFullscreen");

			#endregion

		}

		public static class Mouse
		{
			[Flags]
			public enum Button
			{
				Left = 1 << 0,
				Middle = 1 << 1,
				Right = 1 << 2,
				X1Mask = 1 << 3,
				X2Mask = 1 << 4
			}

			public enum SystemCursor
			{
				Arrow,
				IBeam,
				Wait,
				Crosshair,
				WaitArrow,
				SizeNWSE,
				SizeNESW,
				SizeWE,
				SizeNS,
				SizeAll,
				No,
				Hand
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct MotionEvent
			{
				public EventType Type;
				public uint Timestamp;
				public uint WindowID;
				public uint Which;
				public byte State;
				private byte _padding1;
				private byte _padding2;
				private byte _padding3;
				public int X;
				public int Y;
				public int Xrel;
				public int Yrel;
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct ButtonEvent
			{
				public EventType Type;
				public uint Timestamp;
				public uint WindowID;
				public uint Which;
				public byte Button; /* button id */
				public byte State; /* SDL_PRESSED or SDL_RELEASED */
				public byte Clicks; /* 1 for single-click, 2 for double-click, etc. */
				private byte Padding1;
				public int x;
				public int y;
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct WheelEvent
			{
				public EventType Type;
				public uint TimeStamp;
				public uint WindowId;
				public uint Which;
				public int X;
				public int Y;
				public uint Direction;
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate Button d_sdl_getglobalmousestate(out int x, out int y);
			public static d_sdl_getglobalmousestate GetGlobalState = FuncLoader.LoadFunction<d_sdl_getglobalmousestate>(NativeLibrary, "SDL_GetGlobalMouseState");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate Button d_sdl_getmousestate(out int x, out int y);
			public static d_sdl_getmousestate GetState = FuncLoader.LoadFunction<d_sdl_getmousestate>(NativeLibrary, "SDL_GetMouseState");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate int GetRelativeMouseModeDelegate();
			public static GetRelativeMouseModeDelegate GetRelativeMode = FuncLoader.LoadFunction<GetRelativeMouseModeDelegate>(NativeLibrary, "SDL_GetRelativeMouseMode");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate Button GetRelativeMouseStateDelegate(out int x, out int y);
			public static GetRelativeMouseStateDelegate GetRelativeState = FuncLoader.LoadFunction<GetRelativeMouseStateDelegate>(NativeLibrary, "SDL_GetRelativeMouseState");

		}

		public static class Keyboard
		{
			public struct Keysym
			{
				public int Scancode;
				public int Sym;
				public Keymod Mod;
				public uint Unicode;
			}

			[Flags]
			public enum Keymod : ushort
			{
				None = 0x0000,
				LeftShift = 0x0001,
				RightShift = 0x0002,
				LeftCtrl = 0x0040,
				RightCtrl = 0x0080,
				LeftAlt = 0x0100,
				RightAlt = 0x0200,
				LeftGui = 0x0400,
				RightGui = 0x0800,
				NumLock = 0x1000,
				CapsLock = 0x2000,
				AltGr = 0x4000,
				Reserved = 0x8000,
				Ctrl = (LeftCtrl | RightCtrl),
				Shift = (LeftShift | RightShift),
				Alt = (LeftAlt | RightAlt),
				Gui = (LeftGui | RightGui)
			}

			[StructLayout(LayoutKind.Sequential)]
			public struct Event
			{
				public EventType Type;
				public uint TimeStamp;
				public uint WindowId;
				public byte State;
				public byte Repeat;
				private byte padding2;
				private byte padding3;
				public Keysym Keysym;
			}
		}

		public static class GL
		{
			public enum Attribute
			{
				RedSize,
				GreenSize,
				BlueSize,
				AlphaSize,
				BufferSize,
				DoubleBuffer,
				DepthSize,
				StencilSize,
				AccumRedSize,
				AccumGreenSize,
				AccumBlueSize,
				AccumAlphaSize,
				Stereo,
				MultiSampleBuffers,
				MultiSampleSamples,
				AcceleratedVisual,
				RetainedBacking,
				ContextMajorVersion,
				ContextMinorVersion,
				ContextEgl,
				ContextFlags,
				ContextProfileMask,
				ShareWithCurrentContext,
				FramebufferSRGBCapable,
				ContextReleaseBehaviour,
			}

			[Flags]
			public enum Context
			{
				Debug = 0x0001,
				ForwardCompatible = 0x0002,
				RobustAccess = 0x0004,
				ResetIsolation = 0x0008
			}

			[Flags]
			public enum ContextProfile
			{
				Core = 0x0001,
				Compatibility = 0x0002,
				ES = 0x0004
			}

			public static IntPtr CreateContext(IntPtr window)
			{
				return LogOnError(CreateContextNative(window));
			}

			public static int SetAttribute(Attribute attr, int value)
			{
				return LogOnError(SetAttributeNative(attr, value));
			}

			public static int GetAttribute(Attribute attr, out int value)
			{
				return LogOnError(GetAttributeNative(attr, out value));
			}

			#region Delagetes

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void SwapWindowFunc(IntPtr window);
			public static SwapWindowFunc SwapWindow = FuncLoader.LoadFunction<SwapWindowFunc>(NativeLibrary, "SDL_GL_SwapWindow");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate IntPtr GetProcAddressFunc(string proc);
			public static GetProcAddressFunc GetProcAddress = FuncLoader.LoadFunction<GetProcAddressFunc>(NativeLibrary, "SDL_GL_GetProcAddress");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void DeleteContextDelegate(IntPtr context);
			public static DeleteContextDelegate DeleteContext = FuncLoader.LoadFunction<DeleteContextDelegate>(NativeLibrary, "SDL_GL_DeleteContext");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate void GetDrawableSizeDelegate(IntPtr context, out int w, out int h);
			public static GetDrawableSizeDelegate GetDrawableSize = FuncLoader.LoadFunction<GetDrawableSizeDelegate>(NativeLibrary, "SDL_GL_GetDrawableSize");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			public delegate int d_sdl_gl_setswapinterval(int interval);
			public static d_sdl_gl_setswapinterval SetSwapInterval = FuncLoader.LoadFunction<d_sdl_gl_setswapinterval>(NativeLibrary, "SDL_GL_SetSwapInterval");


			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate IntPtr CreateContextFunc(IntPtr window);
			private static readonly CreateContextFunc CreateContextNative = FuncLoader.LoadFunction<CreateContextFunc>(NativeLibrary, "SDL_GL_CreateContext");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate int SetAttributeFunc(Attribute attr, int value);
			private static readonly SetAttributeFunc SetAttributeNative = FuncLoader.LoadFunction<SetAttributeFunc>(NativeLibrary, "SDL_GL_SetAttribute");

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate int GetAttributeFunc(Attribute attr, out int value);
			private static readonly GetAttributeFunc GetAttributeNative = FuncLoader.LoadFunction<GetAttributeFunc>(NativeLibrary, "SDL_GL_GetAttribute");

			#endregion
		}

		private static IntPtr GetNativeLibrary()
		{
			var result = IntPtr.Zero;

			var assemblyLocation = Path.GetDirectoryName(typeof(Sdl).Assembly.Location);

			if (Environment.OSVersion.IsWindows())
			{
				result = FuncLoader.LoadLibrary(Environment.Is64BitProcess
					                                       ? Path.Combine(assemblyLocation, "x64/SDL2.dll")
					                                       : Path.Combine(assemblyLocation, "x86/SDL2.dll"));
			}
            else if (Environment.OSVersion.IsMacOSX())
            {
                result = FuncLoader.LoadLibrary(Path.Combine(assemblyLocation, "libSDL2-2.0.0.dylib"));
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				result = FuncLoader.LoadLibrary(Environment.Is64BitProcess
					                                       ? Path.Combine(assemblyLocation, "x64/libSDL2-2.0.so.0")
					                                       : Path.Combine(assemblyLocation, "x86/libSDL2-2.0.so.0"));
			}
			

			if (result == IntPtr.Zero)
			{
				if (Environment.OSVersion.IsWindows())
					result = FuncLoader.LoadLibrary("SDL2.dll");
				else if (Environment.OSVersion.Platform == PlatformID.Unix)
					result = FuncLoader.LoadLibrary("libSDL2-2.0.so.0");
				else if (Environment.OSVersion.IsMacOSX())
					result = FuncLoader.LoadLibrary("libSDL2-2.0.0.dylib");
			}

			if (result == IntPtr.Zero)
				throw new Exception("Failed to load SDL library.");

			return result;
		}
	}
}
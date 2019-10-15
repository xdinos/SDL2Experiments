using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Lunatics.Framework.Graphics;
using SDL2;

namespace Lunatics.Framework.Sdl.OpenGL
{
	internal static class GL
	{
		internal enum StringName : int
		{
			Vendor = 0x1f00,
			Renderer = 0x1f01,
			Version = 0x1f02,
			Extensions = 0x1f03,
			ShadingLanguageVersion = 0x8B8C
		}

		internal enum BufferUsageHint
		{
			StreamDraw = 0x88E0,
			StaticDraw = 0x88E4,
		}

		internal enum ShaderType
		{
			VertexShader = 0x8B31,
			FragmentShader = 0x8B30,
		}

		internal enum BufferTarget
		{
			ArrayBuffer = 0x8892,
			ElementArrayBuffer = 0x8893,
		}

		internal enum EnableCap : int
		{
			PointSmooth = 0x0B10,
			LineSmooth = 0x0B20,
			CullFace = 0x0B44,
			Lighting = 0x0B50,
			ColorMaterial = 0x0B57,
			Fog = 0x0B60,
			DepthTest = 0x0B71,
			StencilTest = 0x0B90,
			Normalize = 0x0BA1,
			AlphaTest = 0x0BC0,
			Dither = 0x0BD0,
			Blend = 0x0BE2,
			ColorLogicOp = 0x0BF2,
			ScissorTest = 0x0C11,
			Texture2D = 0x0DE1,
			PolygonOffsetFill = 0x8037,
			RescaleNormal = 0x803A,
			VertexArray = 0x8074,
			NormalArray = 0x8075,
			ColorArray = 0x8076,
			TextureCoordArray = 0x8078,
			Multisample = 0x809D,
			SampleAlphaToCoverage = 0x809E,
			SampleAlphaToOne = 0x809F,
			SampleCoverage = 0x80A0,
			DebugOutputSynchronous = 0x8242,
			DebugOutput = 0x92E0,
		}

		internal enum VertexAttribPointerType
		{
			Float = 0x1406,
			Short = 0x1402,
			UnsignedByte = 0x1401,
			HalfFloat = 0x140B,
		}

		internal enum PrimitiveType
		{
			Points = 0x0000,
			Lines = 0x0001,
			LineStrip = 0x0003,
			Triangles = 0x0004,
			TriangleStrip = 0x0005,
		}

		[Flags]
		internal enum ClearBufferMask
		{
			DepthBufferBit = 0x00000100,
			StencilBufferBit = 0x00000400,
			ColorBufferBit = 0x00004000,
		}

		internal enum ErrorCode
		{
			NoError = 0,
		}

		#region Delegate Bindings

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void EnableVertexAttribArrayDelegate(int attrib);
		internal static EnableVertexAttribArrayDelegate EnableVertexAttribArray;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void DisableVertexAttribArrayDelegate(int attrib);
		internal static DisableVertexAttribArrayDelegate DisableVertexAttribArray;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void ViewportDelegate(int x, int y, int w, int h);
		internal static ViewportDelegate Viewport;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate ErrorCode GetErrorDelegate();
		internal static GetErrorDelegate GetError;

		[System.Security.SuppressUnmanagedCodeSecurity]
		internal delegate void ScissorDelegate(int x, int y, int width, int height);
		internal static ScissorDelegate Scissor;

		[System.Security.SuppressUnmanagedCodeSecurity]
		internal delegate IntPtr GetStringDelegate(StringName param);
		private static GetStringDelegate _getStringInternal;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void ClearDepthDelegate(float depth);
		internal static ClearDepthDelegate ClearDepth;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void ClearDelegate(ClearBufferMask mask);
		internal static ClearDelegate Clear;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void ClearColorDelegate(float red, float green, float blue, float alpha);
		internal static ClearColorDelegate ClearColor;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void ClearStencilDelegate(int stencil);
		internal static ClearStencilDelegate ClearStencil;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate int EnableDelegate(EnableCap cap);
		internal static EnableDelegate Enable;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void DisableDelegate(EnableCap cap);
		internal static DisableDelegate Disable;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void BindBufferDelegate(BufferTarget target, int buffer);
		internal static BindBufferDelegate BindBuffer;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void DrawArraysDelegate(PrimitiveType primitiveType, int offset, int count);
		internal static DrawArraysDelegate DrawArrays;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate int CreateShaderDelegate(ShaderType type);
		internal static CreateShaderDelegate CreateShader;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal unsafe delegate void ShaderSourceDelegate(int shaderId, int count, IntPtr code, int* length);
		internal static ShaderSourceDelegate ShaderSourceInternal;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void CompileShaderDelegate(int shaderId);
		internal static CompileShaderDelegate CompileShader;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void ColorMaskDelegate(bool r, bool g, bool b, bool a);
		internal static ColorMaskDelegate ColorMask;

		// TODO: ...
		//[System.Security.SuppressUnmanagedCodeSecurity]
		//internal delegate void DepthFuncDelegate(DepthFunction function);
		//internal static DepthFuncDelegate DepthFunc;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void DepthMaskDelegate(bool enabled);
		internal static DepthMaskDelegate DepthMask;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void StencilMaskDelegate(int mask);
		internal static StencilMaskDelegate StencilMask;
		
		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void GenBuffersDelegate(int count, out int buffer);
		internal static GenBuffersDelegate GenBuffers;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void BufferDataDelegate(BufferTarget target, IntPtr size, IntPtr n, BufferUsageHint usage);
		internal static BufferDataDelegate BufferData;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void BufferSubDataDelegate(BufferTarget target, IntPtr offset, IntPtr size, IntPtr data);
		internal static BufferSubDataDelegate BufferSubData;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void DeleteBuffersDelegate(int count, [In] [Out] ref int buffer);
		internal static DeleteBuffersDelegate DeleteBuffers;

		[System.Security.SuppressUnmanagedCodeSecurity()]
		internal delegate void VertexAttribPointerDelegate(int location, int elementCount, 
		                                                   VertexAttribPointerType type, bool normalize,
		                                                   int stride, IntPtr data);
		internal static VertexAttribPointerDelegate VertexAttribPointer;

		#endregion

		internal static void LoadEntryPoints()
		{
			if (Viewport == null)
				Viewport = LoadFunction<ViewportDelegate>("glViewport");
			if (Scissor == null)
				Scissor = LoadFunction<ScissorDelegate>("glScissor");
			//if (MakeCurrent == null)
			//	MakeCurrent = LoadFunction<MakeCurrentDelegate>("glMakeCurrent");

			GetError = LoadFunction<GetErrorDelegate>("glGetError");

			EnableVertexAttribArray = LoadFunction<EnableVertexAttribArrayDelegate>("glEnableVertexAttribArray");
			DisableVertexAttribArray = LoadFunction<DisableVertexAttribArrayDelegate>("glDisableVertexAttribArray");

			_getStringInternal = LoadFunction<GetStringDelegate>("glGetString");

			ClearDepth = LoadFunction<ClearDepthDelegate>("glClearDepth")
			             ?? LoadFunction<ClearDepthDelegate>("glClearDepthf");

			Clear = LoadFunction<ClearDelegate>("glClear");
			ClearColor = LoadFunction<ClearColorDelegate>("glClearColor");
			ClearStencil = LoadFunction<ClearStencilDelegate>("glClearStencil");

			Enable = LoadFunction<EnableDelegate>("glEnable");
			Disable = LoadFunction<DisableDelegate>("glDisable");

			BindBuffer = LoadFunction<BindBufferDelegate>("glBindBuffer");
			//DrawBuffers = LoadFunction<DrawBuffersDelegate>("glDrawBuffers");
			//DrawElements = LoadFunction<DrawElementsDelegate>("glDrawElements");
			DrawArrays = LoadFunction<DrawArraysDelegate>("glDrawArrays");
			//Uniform1i = LoadFunction<Uniform1iDelegate>("glUniform1i");
			//Uniform4fv = LoadFunction<Uniform4fvDelegate>("glUniform4fv");
			//ReadPixelsInternal = LoadFunction<ReadPixelsDelegate>("glReadPixels");

			CreateShader = LoadFunction<CreateShaderDelegate>("glCreateShader");
			ShaderSourceInternal = LoadFunction<ShaderSourceDelegate>("glShaderSource");
			CompileShader = LoadFunction<CompileShaderDelegate>("glCompileShader");

			ColorMask = LoadFunction<ColorMaskDelegate>("glColorMask");
			//TODO: DepthFunc = LoadFunction<DepthFuncDelegate>("glDepthFunc");
			DepthMask = LoadFunction<DepthMaskDelegate>("glDepthMask");

			StencilMask = LoadFunction<StencilMaskDelegate>("glStencilMask");

			GenBuffers = LoadFunction<GenBuffersDelegate>("glGenBuffers");
			BufferData = LoadFunction<BufferDataDelegate>("glBufferData");
			//MapBuffer = LoadFunction<MapBufferDelegate>("glMapBuffer");
			//UnmapBuffer = LoadFunction<UnmapBufferDelegate>("glUnmapBuffer");
			BufferSubData = LoadFunction<BufferSubDataDelegate>("glBufferSubData");
			//DeleteBuffers = LoadFunction<DeleteBuffersDelegate>("glDeleteBuffers");

			VertexAttribPointer = LoadFunction<VertexAttribPointerDelegate>("glVertexAttribPointer");
		}

		internal static string GetString(StringName name)
		{
			return Marshal.PtrToStringAnsi(_getStringInternal(name));
		}

		internal static unsafe void ShaderSource(int shaderId, string code)
		{
			int length = code.Length;
			IntPtr intPtr = MarshalStringArrayToPtr(new string[] { code });
			ShaderSourceInternal(shaderId, 1, intPtr, &length);
			FreeStringArrayPtr(intPtr, 1);
		}

		[Conditional("DEBUG")]
		[DebuggerHidden]
		public static void CheckGLError()
		{
			var error = GetError();
			//Console.WriteLine(error);
			if (error != ErrorCode.NoError)
				throw new Exception($"GL.GetError() returned '{error}'");
		}

		private static IntPtr MarshalStringArrayToPtr(string[] strings)
		{
			IntPtr intPtr = IntPtr.Zero;
			if (strings != null && strings.Length != 0)
			{
				intPtr = Marshal.AllocHGlobal(strings.Length * IntPtr.Size);
				if (intPtr == IntPtr.Zero)
				{
					throw new OutOfMemoryException();
				}
				int i = 0;
				try
				{
					for (i = 0; i < strings.Length; i++)
					{
						IntPtr val = MarshalStringToPtr(strings[i]);
						Marshal.WriteIntPtr(intPtr, i * IntPtr.Size, val);
					}
				}
				catch (OutOfMemoryException)
				{
					for (i--; i >= 0; i--)
					{
						Marshal.FreeHGlobal(Marshal.ReadIntPtr(intPtr, i * IntPtr.Size));
					}
					Marshal.FreeHGlobal(intPtr);
					throw;
				}
			}
			return intPtr;
		}
		private static unsafe IntPtr MarshalStringToPtr(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return IntPtr.Zero;
			}
			int num = Encoding.ASCII.GetMaxByteCount(str.Length) + 1;
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			if (intPtr == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			fixed (char* chars = str + RuntimeHelpers.OffsetToStringData / 2)
			{
				int bytes = Encoding.ASCII.GetBytes(chars, str.Length, (byte*)((void*)intPtr), num);
				Marshal.WriteByte(intPtr, bytes, 0);
				return intPtr;
			}
		}

		private static void FreeStringArrayPtr(IntPtr ptr, int length)
		{
			for (int i = 0; i < length; i++)
			{
				Marshal.FreeHGlobal(Marshal.ReadIntPtr(ptr, i * IntPtr.Size));
			}
			Marshal.FreeHGlobal(ptr);
		}

		private static T LoadFunction<T>(string functionName, bool throwIfNotFound = false)
		{
			var address = SDL.SDL_GL_GetProcAddress(functionName);

			if (address != IntPtr.Zero) 
				return Marshal.GetDelegateForFunctionPointer<T>(address);

			if (throwIfNotFound)
				throw new EntryPointNotFoundException(functionName);

			return default(T);
		}
	}
}
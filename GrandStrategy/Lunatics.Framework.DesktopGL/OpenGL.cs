using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Lunatics.Mathematics;

namespace Lunatics.Framework.DesktopGL
{
	public static class OpenGL
	{
		internal enum BufferAccess
		{
			ReadOnly = 0x88B8,
		}

		internal enum BufferUsageHint
		{
			StreamDraw = 0x88E0,
			StaticDraw = 0x88E4,
		}

		internal enum StencilFace
		{
			Front = 0x0404,
			Back = 0x0405,
		}

		internal enum DrawBuffersEnum
		{
			UnsignedShort,
			UnsignedInt,
		}

		internal enum ShaderType
		{
			VertexShader = 0x8B31,
			FragmentShader = 0x8B30,
		}

		internal enum ShaderParameter
		{
			LogLength = 0x8B84,
			CompileStatus = 0x8B81,
			SourceLength = 0x8B88,
		}

		internal enum GetProgramParameterName
		{
			LogLength = 0x8B84,
			LinkStatus = 0x8B82,
			ActiveUniforms = 0x8B86
		}

		internal enum DrawElementsType
		{
			UnsignedShort = 0x1403,
			UnsignedInt = 0x1405,
		}

		internal enum GetPName : int
		{
			ArrayBufferBinding = 0x8894,
			MaxTextureImageUnits = 0x8872,
			MaxVertexAttribs = 0x8869,
			MaxTextureSize = 0x0D33,
			MaxDrawBuffers = 0x8824,
			TextureBinding2D = 0x8069,
			MaxTextureMaxAnisotropyExt = 0x84FF,
			MaxSamples = 0x8D57,

			NumExtensions = 0x821D
		}

		internal enum StringName
		{
			Vendor = 0x1f00,
			Renderer = 0x1f01,
			Version = 0x1f02,
			Extensions = 0x1f03,
			ShadingLanguageVersion = 0x8B8C
		}

		internal enum FramebufferAttachment
		{
			ColorAttachment0 = 0x8CE0,
			ColorAttachment0Ext = 0x8CE0,
			DepthAttachment = 0x8D00,
			StencilAttachment = 0x8D20,
			ColorAttachmentExt = 0x1800,
			DepthAttachementExt = 0x1801,
			StencilAttachmentExt = 0x1802,
		}

		internal enum PrimitiveType
		{
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

		internal enum TextureUnit
		{
			Texture0 = 33984, // 0x000084C0
			Texture1 = 33985, // 0x000084C1
			Texture2 = 33986, // 0x000084C2
			Texture3 = 33987, // 0x000084C3
			Texture4 = 33988, // 0x000084C4
			Texture5 = 33989, // 0x000084C5
			Texture6 = 33990, // 0x000084C6
			Texture7 = 33991, // 0x000084C7
			Texture8 = 33992, // 0x000084C8
			Texture9 = 33993, // 0x000084C9
			Texture10 = 33994, // 0x000084CA
			Texture11 = 33995, // 0x000084CB
			Texture12 = 33996, // 0x000084CC
			Texture13 = 33997, // 0x000084CD
			Texture14 = 33998, // 0x000084CE
			Texture15 = 33999, // 0x000084CF
			Texture16 = 34000, // 0x000084D0
			Texture17 = 34001, // 0x000084D1
			Texture18 = 34002, // 0x000084D2
			Texture19 = 34003, // 0x000084D3
			Texture20 = 34004, // 0x000084D4
			Texture21 = 34005, // 0x000084D5
			Texture22 = 34006, // 0x000084D6
			Texture23 = 34007, // 0x000084D7
			Texture24 = 34008, // 0x000084D8
			Texture25 = 34009, // 0x000084D9
			Texture26 = 34010, // 0x000084DA
			Texture27 = 34011, // 0x000084DB
			Texture28 = 34012, // 0x000084DC
			Texture29 = 34013, // 0x000084DD
			Texture30 = 34014, // 0x000084DE
			Texture31 = 34015, // 0x000084DF
		}

		internal enum CullFaceMode
		{
			Back = 0x0405,
			Front = 0x0404,
		}

		internal enum FrontFaceDirection
		{
			Cw = 0x0900,
			Ccw = 0x0901,
		}

		internal enum MaterialFace
		{
			FrontAndBack = 0x0408,
		}

		internal enum PolygonMode
		{
			Fill = 0x1B02,
			Line = 0x1B01,
		}


		internal enum StencilFunction
		{
			Always = 0x0207,
			Equal = 0x0202,
			Greater = 0x0204,
			GreaterEqual = 0x0206,
			Less = 0x0201,
			LessEqual = 0x0203,
			Never = 0x0200,
			NotEqual = 0x0205,
		}

		internal enum StencilOp
		{
			Keep = 0x1E00,
			DecrWrap = 0x8508,
			Decr = 0x1E03,
			Incr = 0x1E02,
			IncrWrap = 0x8507,
			Invert = 0x150A,
			Replace = 0x1E01,
			Zero = 0,
		}

		internal enum TextureParameterName
		{
			TextureMaxAnisotropyExt = 0x84FE,
			TextureBaseLevel = 0x813C,
			TextureMaxLevel = 0x813D,
			TextureMinFilter = 0x2801,
			TextureMagFilter = 0x2800,
			TextureWrapS = 0x2802,
			TextureWrapT = 0x2803,
			TextureWrapR = 0x8072,
			TextureBorderColor = 0x1004,
			TextureLodBias = 0x8501,
			TextureCompareMode = 0x884C,
			TextureCompareFunc = 0x884D,
			GenerateMipmap = 0x8191,
		}

		internal enum TextureTarget
		{
			Texture2D = 0x0DE1,
			Texture3D = 0x806F,
			TextureCubeMap = 0x8513,
			TextureCubeMapPositiveX = 0x8515,
			TextureCubeMapPositiveY = 0x8517,
			TextureCubeMapPositiveZ = 0x8519,
			TextureCubeMapNegativeX = 0x8516,
			TextureCubeMapNegativeY = 0x8518,
			TextureCubeMapNegativeZ = 0x851A,
		}

		internal enum PixelInternalFormat
		{
			Rgba8 = 0x8058,
			Rgb8 = 0x8051,
			Rgb5A1 = 0x8057,

			Rgba = 0x1908,
			Rgb = 0x1907,
			Rgba4 = 0x8056,
			Luminance = 0x1909,
			CompressedRgbS3tcDxt1Ext = 0x83F0,
			CompressedSrgbS3tcDxt1Ext = 0x8C4C,
			CompressedRgbaS3tcDxt1Ext = 0x83F1,
			CompressedRgbaS3tcDxt3Ext = 0x83F2,
			CompressedSrgbAlphaS3tcDxt3Ext = 0x8C4E,
			CompressedRgbaS3tcDxt5Ext = 0x83F3,
			CompressedSrgbAlphaS3tcDxt5Ext = 0x8C4F,
			R32f = 0x822E,
			Rg16= 0x822C,
			Rg16f = 0x822F,
			Rgba16f = 0x881A,
			R16f = 0x822D,
			Rg32f = 0x8230,
			Rgba32f = 0x8814,
			Rg8= 0x822B,
			Rg8i = 0x8237,
			Rgba8i = 0x8D8E,
			Rg16ui = 0x823A,
			Rgba16ui = 0x8D76,
			Rgb10A2ui = 0x906F,
			Rgb10A2Ext=0x8059,
			Rgba16 = 0x805B,
			// PVRTC
			CompressedRgbPvrtc2Bppv1Img = 0x8C01,
			CompressedRgbPvrtc4Bppv1Img = 0x8C00,
			CompressedRgbaPvrtc2Bppv1Img = 0x8C03,
			CompressedRgbaPvrtc4Bppv1Img = 0x8C02,
			// ATITC
			AtcRgbaExplicitAlphaAmd = 0x8C93,
			AtcRgbaInterpolatedAlphaAmd = 0x87EE,
			// ETC1
			Etc1 = 0x8D64,
			Srgb = 0x8C40,
		}

		internal enum PixelFormat
		{
			Rgba = 0x1908,
			Rgb = 0x1907,
			Luminance = 0x1909,
			CompressedTextureFormats = 0x86A3,
			Red = 0x1903,
			Rg = 0x8227,
			Bgra = 0x80E1
		}

		internal enum PixelType
		{
			UnsignedByte = 0x1401,
			UnsignedShort565 = 0x8363,
			UnsignedShort4444 = 0x8033,
			UnsignedShort5551 = 0x8034,
			Float = 0x1406,
			HalfFloat = 0x140B,
			HalfFloatOES = 0x8D61,
			Byte = 0x1400,
			UnsignedShort = 0x1403,
			UnsignedInt1010102 = 0x8036,
		}

		internal enum PixelStoreParameter
		{
			UnpackAlignment = 0x0CF5,
			PackAlignment = 0x0D05,
		}

		internal enum TextureMinFilter
		{
			LinearMipmapNearest = 0x2701,
			NearestMipmapLinear = 0x2702,
			LinearMipmapLinear = 0x2703,
			Linear = 0x2601,
			NearestMipmapNearest = 0x2700,
			Nearest = 0x2600,
		}

		internal enum TextureMagFilter
		{
			Linear = 0x2601,
			Nearest = 0x2600,
		}


		internal enum GenerateMipmapTarget
		{
			Texture1D = 0x0DE0,
			Texture2D = 0x0DE1,
			Texture3D = 0x806F,
			TextureCubeMap = 0x8513,
			Texture1DArray = 0x8C18,
			Texture2DArray = 0x8C1A,
			Texture2DMultisample = 0x9100,
			Texture2DMultisampleArray = 0x9102,
		}

		internal enum BlitFramebufferFilter
		{
			Nearest = 0x2600,
		}

		internal enum ReadBufferMode
		{
			ColorAttachment0 = 0x8CE0,
		}

		internal enum DrawBufferMode
		{
			ColorAttachment0 = 0x8CE0,
		}

		internal enum BufferTarget
		{
			ArrayBuffer = 0x8892,
			ElementArrayBuffer = 0x8893,
		}

		internal enum FramebufferErrorCode
		{
			FramebufferUndefined = 0x8219,
			FramebufferComplete = 0x8CD5,
			FramebufferCompleteExt = 0x8CD5,
			FramebufferIncompleteAttachment = 0x8CD6,
			FramebufferIncompleteAttachmentExt = 0x8CD6,
			FramebufferIncompleteMissingAttachment = 0x8CD7,
			FramebufferIncompleteMissingAttachmentExt = 0x8CD7,
			FramebufferIncompleteDimensionsExt = 0x8CD9,
			FramebufferIncompleteFormatsExt = 0x8CDA,
			FramebufferIncompleteDrawBuffer = 0x8CDB,
			FramebufferIncompleteDrawBufferExt = 0x8CDB,
			FramebufferIncompleteReadBuffer = 0x8CDC,
			FramebufferIncompleteReadBufferExt = 0x8CDC,
			FramebufferUnsupported = 0x8CDD,
			FramebufferUnsupportedExt = 0x8CDD,
			FramebufferIncompleteMultisample = 0x8D56,
			FramebufferIncompleteLayerTargets = 0x8DA8,
			FramebufferIncompleteLayerCount = 0x8DA9,
		}

		internal enum RenderbufferTarget
		{
			Renderbuffer = 0x8D41,
			RenderbufferExt = 0x8D41,
		}

		internal enum FramebufferTarget
		{
			Framebuffer = 0x8D40,
			FramebufferExt = 0x8D40,
			ReadFramebuffer = 0x8CA8,
		}

		internal enum RenderbufferStorage
		{
			Rgba8 = 0x8058,
			DepthComponent16 = 0x81a5,
			DepthComponent24 = 0x81a6,
			Depth24Stencil8 = 0x88F0,

			// GLES Values
			DepthComponent24Oes = 0x81A6,
			Depth24Stencil8Oes = 0x88F0,
			StencilIndex8 = 0x8D48,
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
			SampleMask = 0x8E51,
			DebugOutputSynchronous = 0x8242,
			DebugOutput = 0x92E0,
		}

		internal enum VertexPointerType
		{
			Float = 0x1406,
			Short = 0x1402,
		}

		internal enum VertexAttribPointerType
		{
			Float = 0x1406,
			Short = 0x1402,
			UnsignedByte = 0x1401,
			HalfFloat = 0x140B,
		}

		internal enum BlendEquationMode
		{
			FuncAdd = 0x8006,
			Max = 0x8008, // ios MaxExt
			Min = 0x8007, // ios MinExt
			FuncReverseSubtract = 0x800B,
			FuncSubtract = 0x800A,
		}

		internal enum BlendingFactorSrc
		{
			Zero = 0,
			SrcColor = 0x0300,
			OneMinusSrcColor = 0x0301,
			SrcAlpha = 0x0302,
			OneMinusSrcAlpha = 0x0303,
			DstAlpha = 0x0304,
			OneMinusDstAlpha = 0x0305,
			DstColor = 0x0306,
			OneMinusDstColor = 0x0307,
			SrcAlphaSaturate = 0x0308,
			ConstantColor = 0x8001,
			OneMinusConstantColor = 0x8002,
			ConstantAlpha = 0x8003,
			OneMinusConstantAlpha = 0x8004,
			One = 1,
		}

		internal enum BlendingFactorDest
		{
			Zero = 0,
			SrcColor = 0x0300,
			OneMinusSrcColor = 0x0301,
			SrcAlpha = 0x0302,
			OneMinusSrcAlpha = 0x0303,
			DstAlpha = 0x0304,
			OneMinusDstAlpha = 0x0305,
			DstColor = 0X0306,
			OneMinusDstColor = 0x0307,
			SrcAlphaSaturate = 0x0308,
			ConstantColor = 0x8001,
			OneMinusConstantColor = 0x8002,
			ConstantAlpha = 0x8003,
			OneMinusConstantAlpha = 0x8004,
			One = 1,
		}

		internal enum DepthFunction
		{
			Always = 0x0207,
			Equal = 0x0202,
			Greater = 0x0204,
			GreaterEqual = 0x0206,
			Less = 0x0201,
			LessEqual = 0x0203,
			Never = 0x0200,
			NotEqual = 0x0205,
		}

		internal enum TextureWrapMode
		{
			ClampToEdge = 0x812F,
			Repeat = 0x2901,
			MirroredRepeat = 0x8370,
			//GLES
			ClampToBorder = 0x812D,
		}

		internal static class GL
		{
			public static bool SupportsBaseVertex { get; private set; }

			public static bool SupportsDxt1 { get; private set; }

			public static bool SupportsS3tc { get; private set; }

			#region Delegates

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void EnableVertexAttribArrayDelegate(int attrib);
			internal static EnableVertexAttribArrayDelegate EnableVertexAttribArray;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DisableVertexAttribArrayDelegate(int attrib);
			internal static DisableVertexAttribArrayDelegate DisableVertexAttribArray;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void MakeCurrentDelegate(IntPtr window);
			internal static MakeCurrentDelegate MakeCurrent;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal unsafe delegate void GetIntegerDelegate(int param, [Out] int* data);
			internal static GetIntegerDelegate GetIntegerv;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void ClearDepthDelegate(float depth);
			internal static ClearDepthDelegate ClearDepth;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DepthRangedDelegate(double min, double max);
			internal static DepthRangedDelegate DepthRanged;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DepthRangefDelegate(float min, float max);
			internal static DepthRangefDelegate DepthRangef;

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
			internal delegate void ViewportDelegate(int x, int y, int w, int h);
			internal static ViewportDelegate Viewport;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate ErrorCode GetErrorDelegate();
			internal static GetErrorDelegate GetError;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void FlushDelegate();
			internal static FlushDelegate Flush;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void GenTexturesDelegte(int count, [Out] out int id);
			internal static GenTexturesDelegte GenTextures;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void BindTextureDelegate(TextureTarget target, int id);
			internal static BindTextureDelegate BindTexture;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate int EnableDelegate(EnableCap cap);

			internal static EnableDelegate Enable;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate int DisableDelegate(EnableCap cap);
			internal static DisableDelegate Disable;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void CullFaceDelegate(CullFaceMode mode);
			internal static CullFaceDelegate CullFace;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void FrontFaceDelegate(FrontFaceDirection direction);
			internal static FrontFaceDelegate FrontFace;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void PolygonModeDelegate(MaterialFace face, PolygonMode mode);
			internal static PolygonModeDelegate PolygonMode;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void PolygonOffsetDelegate(float slopeScaleDepthBias, float depthBias);
			internal static PolygonOffsetDelegate PolygonOffset;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DrawBuffersDelegate(int count, DrawBuffersEnum[] buffers);
			internal static DrawBuffersDelegate DrawBuffers;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void UseProgramDelegate(int program);
			internal static UseProgramDelegate UseProgram;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal unsafe delegate void Uniform4fvDelegate(int location, int size, float* values);
			internal static Uniform4fvDelegate Uniform4fv;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void Uniform1iDelegate(int location, int value);
			internal static Uniform1iDelegate Uniform1i;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			private unsafe delegate void UniformMatrix4fvDelegate(int location, int size, bool transpose, float* values);
			private static UniformMatrix4fvDelegate UniformMatrix4fv;
			
			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void ScissorDelegate(int x, int y, int width, int height);
			internal static ScissorDelegate Scissor;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void ReadPixelsDelegate(int x, int y, int width, int height, PixelFormat format, PixelType type, IntPtr data);
			internal static ReadPixelsDelegate ReadPixelsInternal;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void BindBufferDelegate(BufferTarget target, int buffer);
			internal static BindBufferDelegate BindBuffer;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DrawElementsDelegate(PrimitiveType primitiveType, int count, DrawElementsType elementType, IntPtr offset);
			internal static DrawElementsDelegate DrawElements;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DrawArraysDelegate(PrimitiveType primitiveType, int offset, int count);
			internal static DrawArraysDelegate DrawArrays;


			[System.Security.SuppressUnmanagedCodeSecurity()]
			private delegate IntPtr GetStringDelegate(StringName param);
			private static GetStringDelegate GetStringInternal;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			private delegate IntPtr GetStringIDelegate(StringName param, uint index);
			private static GetStringIDelegate GetStringIInternal;
			private static string GetStringI(StringName param, uint index)
			{
				unsafe
				{
					return new string((sbyte*)GetStringIInternal(param, index));
				}
			}

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void GenRenderbuffersDelegate(int count, [Out] out int buffer);

			internal static GenRenderbuffersDelegate GenRenderbuffers;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void BindRenderbufferDelegate(RenderbufferTarget target, int buffer);

			internal static BindRenderbufferDelegate BindRenderbuffer;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DeleteRenderbuffersDelegate(int count, [In] [Out] ref int buffer);

			internal static DeleteRenderbuffersDelegate DeleteRenderbuffers;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void RenderbufferStorageMultisampleDelegate(RenderbufferTarget target,
			                                                              int sampleCount,
			                                                              RenderbufferStorage storage,
			                                                              int width,
			                                                              int height);
			internal static RenderbufferStorageMultisampleDelegate RenderbufferStorageMultisample;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void GenFramebuffersDelegate(int count, out int buffer);

			internal static GenFramebuffersDelegate GenFramebuffers;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void BindFramebufferDelegate(FramebufferTarget target, int buffer);

			internal static BindFramebufferDelegate BindFramebuffer;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DeleteFramebuffersDelegate(int count, ref int buffer);

			internal static DeleteFramebuffersDelegate DeleteFramebuffers;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			public delegate void InvalidateFramebufferDelegate(FramebufferTarget target,
			                                                   int numAttachments,
			                                                   FramebufferAttachment[] attachments);

			public static InvalidateFramebufferDelegate InvalidateFramebuffer;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void FramebufferTexture2DDelegate(FramebufferTarget target,
			                                                    FramebufferAttachment attachment,
			                                                    TextureTarget textureTarget,
			                                                    int texture,
			                                                    int level);
			internal static FramebufferTexture2DDelegate FramebufferTexture2D;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void FramebufferTexture2DMultiSampleDelegate(FramebufferTarget target,
			                                                               FramebufferAttachment attachment,
			                                                               TextureTarget textureTarget,
			                                                               int texture,
			                                                               int level,
			                                                               int samples);

			internal static FramebufferTexture2DMultiSampleDelegate FramebufferTexture2DMultiSample;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void FramebufferRenderbufferDelegate(FramebufferTarget target,
			                                                       FramebufferAttachment attachment,
			                                                       RenderbufferTarget renderBufferTarget,
			                                                       int buffer);

			internal static FramebufferRenderbufferDelegate FramebufferRenderbuffer;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			public delegate void RenderbufferStorageDelegate(RenderbufferTarget target,
			                                                 RenderbufferStorage storage,
			                                                 int width,
			                                                 int height);

			public static RenderbufferStorageDelegate RenderbufferStorage;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void GenerateMipmapDelegate(GenerateMipmapTarget target);
			internal static GenerateMipmapDelegate GenerateMipmap;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void ReadBufferDelegate(ReadBufferMode buffer);
			internal static ReadBufferDelegate ReadBuffer;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DrawBufferDelegate(DrawBufferMode buffer);
			internal static DrawBufferDelegate DrawBuffer;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void BlitFramebufferDelegate(int srcX0,
			                                               int srcY0,
			                                               int srcX1,
			                                               int srcY1,
			                                               int dstX0,
			                                               int dstY0,
			                                               int dstX1,
			                                               int dstY1,
			                                               ClearBufferMask mask,
			                                               BlitFramebufferFilter filter);
			internal static BlitFramebufferDelegate BlitFramebuffer;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate FramebufferErrorCode CheckFramebufferStatusDelegate(FramebufferTarget target);
			internal static CheckFramebufferStatusDelegate CheckFramebufferStatus;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void ActiveTextureDelegate(TextureUnit textureUnit);
			internal static ActiveTextureDelegate ActiveTexture;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate int CreateShaderDelegate(ShaderType type);
			internal static CreateShaderDelegate CreateShader;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			private unsafe delegate void ShaderSourceDelegate(int shaderId, int count, IntPtr code, int* length);
			private static ShaderSourceDelegate ShaderSourceInternal;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void CompileShaderDelegate(int shaderId);
			internal static CompileShaderDelegate CompileShader;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal unsafe delegate void GetShaderDelegate(int shaderId, int parameter, int* value);
			internal static GetShaderDelegate GetShaderiv;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			private delegate void GetShaderInfoLogDelegate(int shader, int bufSize, IntPtr length, StringBuilder infoLog);
			private static GetShaderInfoLogDelegate GetShaderInfoLogInternal;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate bool IsShaderDelegate(int shaderId);
			internal static IsShaderDelegate IsShader;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DeleteShaderDelegate(int shaderId);
			internal static DeleteShaderDelegate DeleteShader;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate int GetAttribLocationDelegate(int programId, string name);
			internal static GetAttribLocationDelegate GetAttribLocation;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate int GetUniformLocationDelegate(int programId, string name);
			internal static GetUniformLocationDelegate GetUniformLocation;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate bool IsProgramDelegate(int programId);
			internal static IsProgramDelegate IsProgram;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DeleteProgramDelegate(int programId);
			internal static DeleteProgramDelegate DeleteProgram;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate int CreateProgramDelegate();
			internal static CreateProgramDelegate CreateProgram;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void AttachShaderDelegate(int programId, int shaderId);
			internal static AttachShaderDelegate AttachShader;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void LinkProgramDelegate(int programId);
			internal static LinkProgramDelegate LinkProgram;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal unsafe delegate void GetProgramDelegate(int programId, int name, int* linked);
			internal static GetProgramDelegate GetProgramiv;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void GetProgramInfoLogDelegate(int program, int bufSize, IntPtr length, StringBuilder infoLog);
			internal static GetProgramInfoLogDelegate GetProgramInfoLogInternal;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DetachShaderDelegate(int programId, int shaderId);
			internal static DetachShaderDelegate DetachShader;


			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void BlendColorDelegate(float r, float g, float b, float a);
			internal static BlendColorDelegate BlendColor;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void BlendEquationSeparateDelegate(BlendEquationMode colorMode,
			                                                     BlendEquationMode alphaMode);
			internal static BlendEquationSeparateDelegate BlendEquationSeparate;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void BlendEquationSeparateiDelegate(int buffer,
			                                                      BlendEquationMode colorMode,
			                                                      BlendEquationMode alphaMode);
			internal static BlendEquationSeparateiDelegate BlendEquationSeparatei;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void BlendFuncSeparateDelegate(BlendingFactorSrc colorSrc,
			                                                 BlendingFactorDest colorDst,
			                                                 BlendingFactorSrc alphaSrc,
			                                                 BlendingFactorDest alphaDst);

			internal static BlendFuncSeparateDelegate BlendFuncSeparate;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void BlendFuncSeparateiDelegate(int buffer,
			                                                  BlendingFactorSrc colorSrc,
			                                                  BlendingFactorDest colorDst,
			                                                  BlendingFactorSrc alphaSrc,
			                                                  BlendingFactorDest alphaDst);

			internal static BlendFuncSeparateiDelegate BlendFuncSeparatei;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void ColorMaskDelegate(bool r, bool g, bool b, bool a);

			internal static ColorMaskDelegate ColorMask;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DepthFuncDelegate(DepthFunction function);

			internal static DepthFuncDelegate DepthFunc;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DepthMaskDelegate(bool enabled);
			internal static DepthMaskDelegate DepthMask;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void StencilFuncSeparateDelegate(StencilFace face,
			                                                   StencilFunction function,
			                                                   int referenceStencil,
			                                                   int mask);

			internal static StencilFuncSeparateDelegate StencilFuncSeparate;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void StencilOpSeparateDelegate(StencilFace face,
			                                                 StencilOp stencilfail,
			                                                 StencilOp depthFail,
			                                                 StencilOp pass);

			internal static StencilOpSeparateDelegate StencilOpSeparate;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void StencilFuncDelegate(StencilFunction function, int referenceStencil, int mask);

			internal static StencilFuncDelegate StencilFunc;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void StencilOpDelegate(StencilOp stencilFail, StencilOp depthFail, StencilOp pass);

			internal static StencilOpDelegate StencilOp;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void StencilMaskDelegate(int mask);
			internal static StencilMaskDelegate StencilMask;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void TexParameterFloatDelegate(TextureTarget target, TextureParameterName name, float value);
			internal static TexParameterFloatDelegate TexParameterf;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal unsafe delegate void TexParameterFloatArrayDelegate(TextureTarget target, TextureParameterName name, float* values);
			internal static TexParameterFloatArrayDelegate TexParameterfv;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void TexParameterIntDelegate(TextureTarget target, TextureParameterName name, int value);
			internal static TexParameterIntDelegate TexParameteri;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void CompressedTexImage2DDelegate(TextureTarget target, int level, PixelInternalFormat internalFormat,
			                                                    int width, int height, int border, int size, IntPtr data);
			internal static CompressedTexImage2DDelegate CompressedTexImage2D;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void TexImage2DDelegate(TextureTarget target, int level, PixelInternalFormat internalFormat,
			                                          int width, int height, int border, PixelFormat format, PixelType pixelType, IntPtr data);
			internal static TexImage2DDelegate TexImage2D;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void CompressedTexSubImage2DDelegate(TextureTarget target, int level,
		   int x, int y, int width, int height, PixelInternalFormat format, int size, IntPtr data);
			internal static CompressedTexSubImage2DDelegate CompressedTexSubImage2D;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void TexSubImage2DDelegate(TextureTarget target, int level,
				int x, int y, int width, int height, PixelFormat format, PixelType pixelType, IntPtr data);
			internal static TexSubImage2DDelegate TexSubImage2D;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void PixelStoreDelegate(PixelStoreParameter parameter, int size);
			internal static PixelStoreDelegate PixelStore;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void FinishDelegate();
			internal static FinishDelegate Finish;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void GetTexImageDelegate(TextureTarget target, int level, PixelFormat format, PixelType type, [Out] IntPtr pixels);
			internal static GetTexImageDelegate GetTexImageInternal;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void GetCompressedTexImageDelegate(TextureTarget target, int level, [Out] IntPtr pixels);
			internal static GetCompressedTexImageDelegate GetCompressedTexImageInternal;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void TexImage3DDelegate(TextureTarget target, int level, PixelInternalFormat internalFormat,
				int width, int height, int depth, int border, PixelFormat format, PixelType pixelType, IntPtr data);
			internal static TexImage3DDelegate TexImage3D;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void TexSubImage3DDelegate(TextureTarget target, int level,
				int x, int y, int z, int width, int height, int depth, PixelFormat format, PixelType pixelType, IntPtr data);
			internal static TexSubImage3DDelegate TexSubImage3D;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DeleteTexturesDelegate(int count, ref int id);
			internal static DeleteTexturesDelegate DeleteTextures;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void GenBuffersDelegate(int count, out int buffer);
			internal static GenBuffersDelegate GenBuffers;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void BufferDataDelegate(BufferTarget target, IntPtr size, IntPtr n, BufferUsageHint usage);
			internal static BufferDataDelegate BufferData;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate IntPtr MapBufferDelegate(BufferTarget target, BufferAccess access);
			internal static MapBufferDelegate MapBuffer;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void UnmapBufferDelegate(BufferTarget target);
			internal static UnmapBufferDelegate UnmapBuffer;

			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			internal delegate void GetBufferSubDataDelegate(BufferTarget target, IntPtr offset, IntPtr size, IntPtr data);
			internal static GetBufferSubDataDelegate GetBufferSubData;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void BufferSubDataDelegate(BufferTarget target, IntPtr offset, IntPtr size, IntPtr data);
			internal static BufferSubDataDelegate BufferSubData;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void DeleteBuffersDelegate(int count, [In] [Out] ref int buffer);
			internal static DeleteBuffersDelegate DeleteBuffers;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void VertexAttribPointerDelegate(int location, int elementCount, VertexAttribPointerType type, bool normalize,
			                                                   int stride, IntPtr data);
			internal static VertexAttribPointerDelegate VertexAttribPointer;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			internal delegate void ColorMaskIDelegate(uint buf, bool red, bool green, bool blue, bool alpha);
			internal static ColorMaskIDelegate ColorMaskI;

			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			internal delegate void SampleMaskIDelegate(uint maskNumber, uint mask);
			internal static SampleMaskIDelegate SampleMaskI;

			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			internal delegate void DrawRangeElementsDelegate(PrimitiveType mode, int start, int end, int count, DrawElementsType type,IntPtr indices);

			internal static DrawRangeElementsDelegate DrawRangeElements;

			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			internal delegate void DrawRangeElementsBaseVertexDelegate(PrimitiveType mode, int start, int end, int count, DrawElementsType type, IntPtr indices, int baseVertex);
			internal static DrawRangeElementsBaseVertexDelegate DrawRangeElementsBaseVertex;

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            internal delegate void GenVertexArraysDelegate(int n, out uint arrays);
            internal static GenVertexArraysDelegate GenVertexArrays;

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            internal delegate void DeleteVertexArraysDelegate(int n, ref uint arrays);
            internal static DeleteVertexArraysDelegate DeleteVertexArrays;

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            internal delegate void BindVertexArrayDelegate(uint arrays);
            internal static BindVertexArrayDelegate BindVertexArray;

            [System.Diagnostics.Conditional("DEBUG")]
			[System.Diagnostics.DebuggerHidden]
			public static void CheckError()
			{
				var error = GetError();
				if (error != ErrorCode.NoError)
                    System.Diagnostics.Debug.WriteLine($"OpenGL.GL.GetError() returned {error}");
                //	throw new Exception($"OpenGL.GL.GetError() returned {error}");
            }

#if DEBUG

			private enum DebugSource
			{
				GL_DEBUG_SOURCE_API = 0x8246,
				GL_DEBUG_SOURCE_WINDOW_SYSTEM = 0x8247,
				GL_DEBUG_SOURCE_SHADER_COMPILER = 0x8248,
				GL_DEBUG_SOURCE_THIRD_PARTY = 0x8249,
				GL_DEBUG_SOURCE_APPLICATION = 0x824A,
				GL_DEBUG_SOURCE_OTHER = 0x824B,
			}

			private enum DebugType
			{
				GL_DEBUG_TYPE_ERROR = 0x824C,
				GL_DEBUG_TYPE_DEPRECATED_BEHAVIOR = 0x824D,
				GL_DEBUG_TYPE_UNDEFINED_BEHAVIOR = 0x824E,
				GL_DEBUG_TYPE_PORTABILITY = 0x824F,
				GL_DEBUG_TYPE_PERFORMANCE = 0x8250,
				GL_DEBUG_TYPE_OTHER = 0x8251,
			}

			private enum DebugSeverity
			{
				GL_DEBUG_SEVERITY_HIGH = 0x9146,
				GL_DEBUG_SEVERITY_MEDIUM = 0x9147,
				GL_DEBUG_SEVERITY_LOW = 0x9148,
				GL_DEBUG_SEVERITY_NOTIFICATION = 0x826B
			}

			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			delegate void DebugMessageCallbackProc(DebugSource source,
			                                       DebugType type,
			                                       int id,
			                                       DebugSeverity severity,
			                                       int length,
			                                       IntPtr message,
			                                       IntPtr userParam);
			private static DebugMessageCallbackProc DebugProc;

			[System.Security.SuppressUnmanagedCodeSecurity()]
			delegate void DebugMessageCallbackDelegate(DebugMessageCallbackProc callback, IntPtr userParam);
			static DebugMessageCallbackDelegate DebugMessageCallback;

			[UnmanagedFunctionPointer(CallingConvention.StdCall)]
			private delegate void DebugMessageControlDelegate(DebugSource source,
			                                                  DebugType type,
			                                                  DebugSeverity severity,
			                                                  int count,
			                                                  IntPtr ids, // const GLuint*
			                                                  bool enabled);
			private static DebugMessageControlDelegate DebugMessageControl;

			internal delegate void ErrorDelegate(string message);
			internal static event ErrorDelegate OnError;

			static void DebugMessageCallbackHandler(DebugSource source,
			                                        DebugType type,
			                                        int id,
			                                        DebugSeverity severity,
			                                        int length,
			                                        IntPtr message,
			                                        IntPtr userParam)
			{
				var errorMessage = $"{Marshal.PtrToStringAnsi(message)}\n\tSource: {source}\n\tType: {type}\n\tSeverity: {severity}";
				System.Diagnostics.Debug.WriteLine(errorMessage);
				OnError?.Invoke(errorMessage);

				if (type == DebugType.GL_DEBUG_TYPE_ERROR)
					throw new InvalidOperationException(errorMessage);

			}
#endif

			#endregion

			internal static string GetString(StringName name)
			{
				return Marshal.PtrToStringAnsi(GetStringInternal(name));
			}

			internal static unsafe void GetInteger(GetPName name, out int value)
			{
				fixed (int* ptr = &value)
				{
					GetIntegerv((int)name, ptr);
				}
			}

			internal static unsafe void GetInteger(int name, out int value)
			{
				fixed (int* ptr = &value)
				{
					GetIntegerv(name, ptr);
				}
			}

			internal static void DepthRange(double min, double max)
			{
				//if (BoundApi == RenderApi.ES)
				//	DepthRangef(min, max);
				//else
					DepthRanged(min, max);
			}

			internal static void TexParameter(TextureTarget target, TextureParameterName name, float value)
			{
				TexParameterf(target, name, value);
			}

			internal static unsafe void TexParameter(TextureTarget target, TextureParameterName name, float[] values)
			{
				fixed (float* ptr = &values[0])
				{
					TexParameterfv(target, name, ptr);
				}
			}

			internal static void TexParameter(TextureTarget target, TextureParameterName name, int value)
			{
				TexParameteri(target, name, value);
			}

			internal static void GetTexImage<T>(TextureTarget target, int level, PixelFormat format, PixelType type, T[] pixels) where T : struct
			{
				var pixelsPtr = GCHandle.Alloc(pixels, GCHandleType.Pinned);
				try
				{
					GetTexImageInternal(target, level, format, type, pixelsPtr.AddrOfPinnedObject());
				}
				finally
				{
					pixelsPtr.Free();
				}
			}

			internal static void GetCompressedTexImage<T>(TextureTarget target, int level, T[] pixels) where T : struct
			{
				var pixelsPtr = GCHandle.Alloc(pixels, GCHandleType.Pinned);
				try
				{
					GetCompressedTexImageInternal(target, level, pixelsPtr.AddrOfPinnedObject());
				}
				finally
				{
					pixelsPtr.Free();
				}
			}

			public static void ReadPixels<T>(int x, int y, int width, int height, PixelFormat format, PixelType type, T[] data)
			{
				var dataPtr = GCHandle.Alloc(data, GCHandleType.Pinned);
				try
				{
					ReadPixelsInternal(x, y, width, height, format, type, dataPtr.AddrOfPinnedObject());
				}
				finally
				{
					dataPtr.Free();
				}
			}

			internal static unsafe void GetShader(int shaderId, ShaderParameter name, out int result)
			{
				fixed (int* ptr = &result)
				{
					GetShaderiv(shaderId, (int)name, ptr);
				}
			}

			internal static unsafe void GetProgram(int programId, GetProgramParameterName name, out int result)
			{
				fixed (int* ptr = &result)
				{
					GetProgramiv(programId, (int)name, ptr);
				}
			}

			internal static string GetProgramInfoLog(int programId)
			{
				GetProgram(programId, GetProgramParameterName.LogLength, out var length);
				var sb = new StringBuilder(length);
				GetProgramInfoLogInternal(programId, length, IntPtr.Zero, sb);
				return sb.ToString();
			}

			internal static string GetShaderInfoLog(int shaderId)
			{
				GetShader(shaderId, ShaderParameter.LogLength, out var length);
				var sb = new StringBuilder(length);
				GetShaderInfoLogInternal(shaderId, length, IntPtr.Zero, sb);
				return sb.ToString();
			}

			internal static unsafe void ShaderSource(int shaderId, string code)
			{
				var length = code.Length;
				IntPtr intPtr = MarshalStringArrayToPtr(new [] { code });
				ShaderSourceInternal(shaderId, 1, intPtr, &length);
				FreeStringArrayPtr(intPtr, 1);
			}

			internal static unsafe void UniformMatrix4(int location, int size, bool transpose, ref Matrix matrix)
			{
				var array = matrix.ToArray();
				fixed (float* ptr = &array[0])
				{
					UniformMatrix4fv(location, size, transpose, ptr);
				}
			}


			internal static void LoadEntryPoints(bool useCoreProfile)
			{
				if (Viewport == null)
					Viewport = LoadFunction<ViewportDelegate>("glViewport");
				if (Scissor == null)
					Scissor = LoadFunction<ScissorDelegate>("glScissor");
				if (MakeCurrent == null)
					MakeCurrent = LoadFunction<MakeCurrentDelegate>("glMakeCurrent");

				GetError = LoadFunction<GetErrorDelegate>("glGetError");

				TexParameterf = LoadFunction<TexParameterFloatDelegate>("glTexParameterf");
				TexParameterfv = LoadFunction<TexParameterFloatArrayDelegate>("glTexParameterfv");
				TexParameteri = LoadFunction<TexParameterIntDelegate>("glTexParameteri");

				GetStringInternal = LoadFunction<GetStringDelegate>("glGetString");

				EnableVertexAttribArray = LoadFunction<EnableVertexAttribArrayDelegate>("glEnableVertexAttribArray");
				DisableVertexAttribArray = LoadFunction<DisableVertexAttribArrayDelegate>("glDisableVertexAttribArray");
				GetIntegerv = LoadFunction<GetIntegerDelegate>("glGetIntegerv");
				GetStringInternal = LoadFunction<GetStringDelegate>("glGetString");
				ClearDepth = LoadFunction<ClearDepthDelegate>("glClearDepth");
				if (ClearDepth == null) ClearDepth = LoadFunction<ClearDepthDelegate>("glClearDepthf");
				DepthRanged = LoadFunction<DepthRangedDelegate>("glDepthRange");
				DepthRangef = LoadFunction<DepthRangefDelegate>("glDepthRangef");
				Clear = LoadFunction<ClearDelegate>("glClear");
				ClearColor = LoadFunction<ClearColorDelegate>("glClearColor");
				ClearStencil = LoadFunction<ClearStencilDelegate>("glClearStencil");
				Flush = LoadFunction<FlushDelegate>("glFlush");
				GenTextures = LoadFunction<GenTexturesDelegte>("glGenTextures");
				BindTexture = LoadFunction<BindTextureDelegate>("glBindTexture");

				Enable = LoadFunction<EnableDelegate>("glEnable");
				Disable = LoadFunction<DisableDelegate>("glDisable");
				CullFace = LoadFunction<CullFaceDelegate>("glCullFace");
				FrontFace = LoadFunction<FrontFaceDelegate>("glFrontFace");
				PolygonMode = LoadFunction<PolygonModeDelegate>("glPolygonMode");
				PolygonOffset = LoadFunction<PolygonOffsetDelegate>("glPolygonOffset");

				BindBuffer = LoadFunction<BindBufferDelegate>("glBindBuffer");
				DrawBuffers = LoadFunction<DrawBuffersDelegate>("glDrawBuffers");
				DrawElements = LoadFunction<DrawElementsDelegate>("glDrawElements");
				DrawArrays = LoadFunction<DrawArraysDelegate>("glDrawArrays");
				Uniform1i = LoadFunction<Uniform1iDelegate>("glUniform1i");
				Uniform4fv = LoadFunction<Uniform4fvDelegate>("glUniform4fv");
				UniformMatrix4fv = LoadFunction<UniformMatrix4fvDelegate>("glUniformMatrix4fv");
				ReadPixelsInternal = LoadFunction<ReadPixelsDelegate>("glReadPixels");

				ReadBuffer = LoadFunction<ReadBufferDelegate>("glReadBuffer");
				DrawBuffer = LoadFunction<DrawBufferDelegate>("glDrawBuffer");

				// Render Target Support. These might be null if they are not supported
				// see GraphicsDevice.OpenGL.FramebufferHelper.cs for handling other extensions.
				GenRenderbuffers = LoadFunction<GenRenderbuffersDelegate>("glGenRenderbuffers");
				BindRenderbuffer = LoadFunction<BindRenderbufferDelegate>("glBindRenderbuffer");
				DeleteRenderbuffers = LoadFunction<DeleteRenderbuffersDelegate>("glDeleteRenderbuffers");
				GenFramebuffers = LoadFunction<GenFramebuffersDelegate>("glGenFramebuffers");
				BindFramebuffer = LoadFunction<BindFramebufferDelegate>("glBindFramebuffer");
				DeleteFramebuffers = LoadFunction<DeleteFramebuffersDelegate>("glDeleteFramebuffers");
				FramebufferTexture2D = LoadFunction<FramebufferTexture2DDelegate>("glFramebufferTexture2D");
				FramebufferRenderbuffer = LoadFunction<FramebufferRenderbufferDelegate>("glFramebufferRenderbuffer");
				RenderbufferStorage = LoadFunction<RenderbufferStorageDelegate>("glRenderbufferStorage");
				RenderbufferStorageMultisample = LoadFunction<RenderbufferStorageMultisampleDelegate>("glRenderbufferStorageMultisample");
				GenerateMipmap = LoadFunction<GenerateMipmapDelegate>("glGenerateMipmap");
				BlitFramebuffer = LoadFunction<BlitFramebufferDelegate>("glBlitFramebuffer");
				CheckFramebufferStatus = LoadFunction<CheckFramebufferStatusDelegate>("glCheckFramebufferStatus");

				ActiveTexture = LoadFunction<ActiveTextureDelegate>("glActiveTexture");
				CreateShader = LoadFunction<CreateShaderDelegate>("glCreateShader");
				ShaderSourceInternal = LoadFunction<ShaderSourceDelegate>("glShaderSource");
				CompileShader = LoadFunction<CompileShaderDelegate>("glCompileShader");
				GetShaderiv = LoadFunction<GetShaderDelegate>("glGetShaderiv");
				GetShaderInfoLogInternal = LoadFunction<GetShaderInfoLogDelegate>("glGetShaderInfoLog");
				IsShader = LoadFunction<IsShaderDelegate>("glIsShader");
				DeleteShader = LoadFunction<DeleteShaderDelegate>("glDeleteShader");
				GetAttribLocation = LoadFunction<GetAttribLocationDelegate>("glGetAttribLocation");
				GetUniformLocation = LoadFunction<GetUniformLocationDelegate>("glGetUniformLocation");

				IsProgram = LoadFunction<IsProgramDelegate>("glIsProgram");
				DeleteProgram = LoadFunction<DeleteProgramDelegate>("glDeleteProgram");
				CreateProgram = LoadFunction<CreateProgramDelegate>("glCreateProgram");
				AttachShader = LoadFunction<AttachShaderDelegate>("glAttachShader");
				UseProgram = LoadFunction<UseProgramDelegate>("glUseProgram");
				LinkProgram = LoadFunction<LinkProgramDelegate>("glLinkProgram");
				GetProgramiv = LoadFunction<GetProgramDelegate>("glGetProgramiv");
				GetProgramInfoLogInternal = LoadFunction<GetProgramInfoLogDelegate>("glGetProgramInfoLog");
				DetachShader = LoadFunction<DetachShaderDelegate>("glDetachShader");

				BlendColor = LoadFunction<BlendColorDelegate>("glBlendColor");
				BlendEquationSeparate = LoadFunction<BlendEquationSeparateDelegate>("glBlendEquationSeparate");
				BlendEquationSeparatei = LoadFunction<BlendEquationSeparateiDelegate>("glBlendEquationSeparatei");
				BlendFuncSeparate = LoadFunction<BlendFuncSeparateDelegate>("glBlendFuncSeparate");
				BlendFuncSeparatei = LoadFunction<BlendFuncSeparateiDelegate>("glBlendFuncSeparatei");
				ColorMask = LoadFunction<ColorMaskDelegate>("glColorMask");
				DepthFunc = LoadFunction<DepthFuncDelegate>("glDepthFunc");
				DepthMask = LoadFunction<DepthMaskDelegate>("glDepthMask");
				StencilFuncSeparate = LoadFunction<StencilFuncSeparateDelegate>("glStencilFuncSeparate");
				StencilOpSeparate = LoadFunction<StencilOpSeparateDelegate>("glStencilOpSeparate");
				StencilFunc = LoadFunction<StencilFuncDelegate>("glStencilFunc");
				StencilOp = LoadFunction<StencilOpDelegate>("glStencilOp");
				StencilMask = LoadFunction<StencilMaskDelegate>("glStencilMask");

				CompressedTexImage2D = LoadFunction<CompressedTexImage2DDelegate>("glCompressedTexImage2D");
				TexImage2D = LoadFunction<TexImage2DDelegate>("glTexImage2D");
				CompressedTexSubImage2D = LoadFunction<CompressedTexSubImage2DDelegate>("glCompressedTexSubImage2D");
				TexSubImage2D = LoadFunction<TexSubImage2DDelegate>("glTexSubImage2D");
				PixelStore = LoadFunction<PixelStoreDelegate>("glPixelStorei");
				Finish = LoadFunction<FinishDelegate>("glFinish");
				GetTexImageInternal = LoadFunction<GetTexImageDelegate>("glGetTexImage");
				GetCompressedTexImageInternal = LoadFunction<GetCompressedTexImageDelegate>("glGetCompressedTexImage");
				TexImage3D = LoadFunction<TexImage3DDelegate>("glTexImage3D");
				TexSubImage3D = LoadFunction<TexSubImage3DDelegate>("glTexSubImage3D");
				DeleteTextures = LoadFunction<DeleteTexturesDelegate>("glDeleteTextures");

				GenBuffers = LoadFunction<GenBuffersDelegate>("glGenBuffers");
				BufferData = LoadFunction<BufferDataDelegate>("glBufferData");
				MapBuffer = LoadFunction<MapBufferDelegate>("glMapBuffer");
				UnmapBuffer = LoadFunction<UnmapBufferDelegate>("glUnmapBuffer");
				GetBufferSubData = LoadFunction<GetBufferSubDataDelegate>("glGetBufferSubData");
				BufferSubData = LoadFunction<BufferSubDataDelegate>("glBufferSubData");
				DeleteBuffers = LoadFunction<DeleteBuffersDelegate>("glDeleteBuffers");

				VertexAttribPointer = LoadFunction<VertexAttribPointerDelegate>("glVertexAttribPointer");

				/* ARB_draw_elements_base_vertex is ideal! */
				var ep = Sdl.GL.GetProcAddress("glDrawRangeElementsBaseVertex");
				if (ep == IntPtr.Zero)
					ep = Sdl.GL.GetProcAddress("glDrawRangeElementsBaseVertexOES");
				SupportsBaseVertex = ep != IntPtr.Zero;
				if (SupportsBaseVertex)
				{
					DrawRangeElementsBaseVertex = Marshal.GetDelegateForFunctionPointer<DrawRangeElementsBaseVertexDelegate>(ep);
					DrawRangeElements = LoadFunction<DrawRangeElementsDelegate>("glDrawRangeElements");
				}
				else
				{
					/* DrawRangeElements is better, and ES3+ should have this */
					ep = Sdl.GL.GetProcAddress("glDrawRangeElements");
					if (ep != IntPtr.Zero)
					{
						DrawRangeElements = Marshal.GetDelegateForFunctionPointer<DrawRangeElementsDelegate>(ep);
						DrawRangeElementsBaseVertex = DrawRangeElementsNoBase;
					}
					else
					{
						try
						{
							DrawElements = LoadFunction<DrawElementsDelegate>("glDrawElements", true);
						}
						catch (Exception e)
						{
							// TODO: throw new NoSuitableGraphicsDeviceException(baseErrorString +"\nEntry Point: " + e.Message +"\n" + driver);
							throw;
						}
						
						DrawRangeElements = DrawRangeElementsUnchecked;
						DrawRangeElementsBaseVertex = DrawRangeElementsNoBaseUnchecked;
					}
				}

                if (useCoreProfile)
                {
					GenVertexArrays = LoadFunction<GenVertexArraysDelegate>("glGenVertexArrays");
					DeleteVertexArrays = LoadFunction<DeleteVertexArraysDelegate>("glDeleteVertexArrays");
					BindVertexArray = LoadFunction<BindVertexArrayDelegate>("glBindVertexArray");
					GetStringIInternal = LoadFunction<GetStringIDelegate>("glGetStringi"); 
				}

                LoadExtensions(useCoreProfile);

                SupportsS3tc = Extensions.Contains("GL_EXT_texture_compression_s3tc") ||
                               Extensions.Contains("GL_OES_texture_compression_S3TC") ||
                               Extensions.Contains("GL_EXT_texture_compression_dxt3") ||
                               Extensions.Contains("GL_EXT_texture_compression_dxt5");
                SupportsDxt1 = SupportsS3tc ||
                               Extensions.Contains("GL_EXT_texture_compression_dxt1");

#if DEBUG
                try
                {
	                // Try KHR_debug first...
	                var debugMessageControlPtr = Sdl.GL.GetProcAddress("glDebugMessageControl");
					var debugMessageCallbackPtr = Sdl.GL.GetProcAddress("glDebugMessageCallback");

					if (debugMessageControlPtr == IntPtr.Zero || 
					    debugMessageCallbackPtr == IntPtr.Zero)
					{
						/* ... then try ARB_debug_output. */
						debugMessageControlPtr = Sdl.GL.GetProcAddress("glDebugMessageControlARB");
						debugMessageCallbackPtr = Sdl.GL.GetProcAddress("glDebugMessageCallbackARB");
					}

					if (debugMessageControlPtr != IntPtr.Zero && debugMessageCallbackPtr != IntPtr.Zero)
					{
						DebugMessageControl= Marshal.GetDelegateForFunctionPointer<DebugMessageControlDelegate>(debugMessageControlPtr);
						DebugMessageCallback = Marshal.GetDelegateForFunctionPointer<DebugMessageCallbackDelegate>(debugMessageCallbackPtr);


						if (DebugMessageCallback != null)
						{
							DebugProc = DebugMessageCallbackHandler;
							DebugMessageCallback(DebugProc, IntPtr.Zero);
							Enable(EnableCap.DebugOutput);
							Enable(EnableCap.DebugOutputSynchronous);
						}
					}
                }
                catch (EntryPointNotFoundException)
                {
	                // Ignore the debug message callback if the entry point can not be found
                }
#endif
			}

			private static T LoadFunction<T>(string function, bool throwIfNotFound = false)
			{
				var ret = Sdl.GL.GetProcAddress(function);

				if (ret == IntPtr.Zero)
				{
					if (throwIfNotFound)
						throw new EntryPointNotFoundException(function);

					return default(T);
				}

				return Marshal.GetDelegateForFunctionPointer<T>(ret);
			}

			private static void LoadExtensions(bool useCoreProfile)
			{
				if (!useCoreProfile)
				{
					var extensionsString = GL.GetString(StringName.Extensions);
					var error = GetError();
					if (!string.IsNullOrEmpty(extensionsString) && error == ErrorCode.NoError)
						Extensions.AddRange(extensionsString.Split(' '));
				}
				else
				{
					GetInteger(GetPName.NumExtensions, out var numExtensions);
					for (uint i = 0; i < numExtensions; i++)
						Extensions.Add(GetStringI(StringName.Extensions,i));
				}
				

				if (GL.GenRenderbuffers == null && Extensions.Contains("GL_EXT_framebuffer_object"))
				{
					LoadFrameBufferObjectEXTEntryPoints();
				}

				if (GL.RenderbufferStorageMultisample == null)
				{
					if (Extensions.Contains("GL_APPLE_framebuffer_multisample"))
					{
						GL.RenderbufferStorageMultisample =
							LoadFunction<GL.RenderbufferStorageMultisampleDelegate>(
								"glRenderbufferStorageMultisampleAPPLE");
						GL.BlitFramebuffer =
							LoadFunction<GL.BlitFramebufferDelegate>("glResolveMultisampleFramebufferAPPLE");
					}
					else if (Extensions.Contains("GL_EXT_multisampled_render_to_texture"))
					{
						GL.RenderbufferStorageMultisample =
							LoadFunction<GL.RenderbufferStorageMultisampleDelegate>(
								"glRenderbufferStorageMultisampleEXT");
						GL.FramebufferTexture2DMultiSample =
							LoadFunction<GL.FramebufferTexture2DMultiSampleDelegate>(
								"glFramebufferTexture2DMultisampleEXT");

					}
					else if (Extensions.Contains("GL_IMG_multisampled_render_to_texture"))
					{
						GL.RenderbufferStorageMultisample =
							LoadFunction<GL.RenderbufferStorageMultisampleDelegate>(
								"glRenderbufferStorageMultisampleIMG");
						GL.FramebufferTexture2DMultiSample =
							LoadFunction<GL.FramebufferTexture2DMultiSampleDelegate>(
								"glFramebufferTexture2DMultisampleIMG");
					}
					else if (Extensions.Contains("GL_NV_framebuffer_multisample"))
					{
						GL.RenderbufferStorageMultisample =
							LoadFunction<GL.RenderbufferStorageMultisampleDelegate>(
								"glRenderbufferStorageMultisampleNV");
						GL.BlitFramebuffer = LoadFunction<GL.BlitFramebufferDelegate>("glBlitFramebufferNV");
					}
				}

				if (GL.BlendFuncSeparatei == null && Extensions.Contains("GL_ARB_draw_buffers_blend"))
				{
					GL.BlendFuncSeparatei = LoadFunction<GL.BlendFuncSeparateiDelegate>("BlendFuncSeparateiARB");
				}

				if (GL.BlendEquationSeparatei == null && Extensions.Contains("GL_ARB_draw_buffers_blend"))
				{
					GL.BlendEquationSeparatei = LoadFunction<GL.BlendEquationSeparateiDelegate>("BlendEquationSeparateiARB");
				}
			}

			private static void LoadFrameBufferObjectEXTEntryPoints()
			{
				GenRenderbuffers = LoadFunction<GenRenderbuffersDelegate>("glGenRenderbuffersEXT");
				BindRenderbuffer = LoadFunction<BindRenderbufferDelegate>("glBindRenderbufferEXT");
				DeleteRenderbuffers = LoadFunction<DeleteRenderbuffersDelegate>("glDeleteRenderbuffersEXT");
				GenFramebuffers = LoadFunction<GenFramebuffersDelegate>("glGenFramebuffersEXT");
				BindFramebuffer = LoadFunction<BindFramebufferDelegate>("glBindFramebufferEXT");
				DeleteFramebuffers = LoadFunction<DeleteFramebuffersDelegate>("glDeleteFramebuffersEXT");
				FramebufferTexture2D = LoadFunction<FramebufferTexture2DDelegate>("glFramebufferTexture2DEXT");
				FramebufferRenderbuffer = LoadFunction<FramebufferRenderbufferDelegate>("glFramebufferRenderbufferEXT");
				RenderbufferStorage = LoadFunction<RenderbufferStorageDelegate>("glRenderbufferStorageEXT");
				RenderbufferStorageMultisample = LoadFunction<RenderbufferStorageMultisampleDelegate>("glRenderbufferStorageMultisampleEXT");
				GenerateMipmap = LoadFunction<GenerateMipmapDelegate>("glGenerateMipmapEXT");
				BlitFramebuffer = LoadFunction<BlitFramebufferDelegate>("glBlitFramebufferEXT");
				CheckFramebufferStatus = LoadFunction<CheckFramebufferStatusDelegate>("glCheckFramebufferStatusEXT");
			}

			private static void DrawRangeElementsNoBase(PrimitiveType mode,
			                                            int start,
			                                            int end,
			                                            int count,
			                                            DrawElementsType type,
			                                            IntPtr indices,
			                                            int baseVertex)
			{
				DrawRangeElements(mode, start, end, count, type, indices);
			}

			private static void DrawRangeElementsNoBaseUnchecked(PrimitiveType mode, int start, int end, int count, DrawElementsType type,IntPtr indices, int baseVertex)
			{
				DrawElements(mode, count, type, indices);
			}

			private static void DrawRangeElementsUnchecked(PrimitiveType mode, int start, int end, int count, DrawElementsType type, IntPtr indices)
			{
				DrawElements(mode, count,type, indices);
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

			private static readonly List<string> Extensions = new List<string>();
		}
	}
}
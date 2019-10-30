using System;
using System.IO;
using System.Net;
using SharpCEGui.Base;
using System.Runtime.InteropServices;
using Lunatics.Mathematics;
using Lunatics.SDLGL;

//#if __MACOS__
//using OpenGL;
//using Icehole.OpenGL;
//#else
//using OpenGL = OpenTK.Graphics.OpenGL;
//#endif

namespace SharpCEGui.OpenGLRenderer
{
    /// <summary>
    /// Texture implementation for the OpenGLRenderer.
    /// </summary>
    public abstract class OpenGLTexture : Texture, IDisposable
    {
        /// <summary>
        /// set the openGL texture that this Texture is based on to the specifiedSystem.Runtime.InteropServices.
        /// texture, with the specified size.
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="size"></param>
        public void SetOpenGLTexture(int tex, Sizef size)
        {
            if (d_ogltexture != tex)
            {
                // cleanup the current state first.
                CleanupOpenGLTexture();

                d_ogltexture = tex;
            }

            d_dataSize = d_size = size;
            UpdateCachedScaleValues();
        }

        /// <summary>
        /// Return the internal OpenGL texture id used by this Texture object.
        /// </summary>
        /// <returns>
        /// id of the OpenGL texture that this object is using.
        /// </returns>
        public int GetOpenGLTexture()
        {
            return d_ogltexture;
        }

        /// <summary>
        /// set the size of the internal texture.
        /// </summary>
        /// <param name="sz">
        /// size for the internal texture, in pixels.
        /// </param>
        /// <remarks>
        /// Depending upon the hardware capabilities, the actual final size of the 
        /// texture may be larger than what is specified when calling this function.
        /// The texture will never be smaller than what you request here.  To
        /// discover the actual size, call getSize.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// thrown if the hardware is unable to support a texture large enough to
        /// fulfill the requested size.
        /// </exception>
        public void SetTextureSize(Sizef sz)
        {
            InitInternalPixelFormatFields(PixelFormat.RGBA);

            SetTextureSizeImpl(sz);

            d_dataSize = d_size;
            UpdateCachedScaleValues();
        }

        /// <summary>
        /// Grab the texture to a local buffer.
        /// 
        /// This will destroy the OpenGL texture, and restoreTexture must be called
        /// before using it again.
        /// </summary>
        public void GrabTexture()
        {
            // if texture has already been grabbed, do nothing.
            if (d_grabBuffer != null)
                return;

            d_grabBuffer = new byte[(int) (4*d_size.Width*d_size.Height)];

            BlitToMemory(d_grabBuffer);

            OpenGL.GL.DeleteTextures(1, ref d_ogltexture);
        }

        /// <summary>
        /// Restore the texture from the locally buffered copy previously create by
        /// a call to grabTexture.
        /// </summary>
        public void RestoreTexture()
        {
            if (d_grabBuffer == null)
                return;

            GenerateOpenGLTexture();
            SetTextureSizeImpl(d_size);

            BlitFromMemory(d_grabBuffer, new Rectf(Vector2.Zero, d_size));

            // free the grabbuffer
            //delete[] d_grabBuffer;
            d_grabBuffer = null;
        }

        public override string GetName()
        {
            return d_name;
        }

        public override Sizef GetSize()
        {
            return d_size;
        }

        public override Sizef GetOriginalDataSize()
        {
            return d_dataSize;
        }

        public override Vector2 GetTexelScaling()
        {
            return d_texelScaling;
        }

        public override void LoadFromFile(string filename, string resourceGroup)
        {
            // Note from PDT:
            // There is somewhat tight coupling here between OpenGLTexture and the
            // ImageCodec classes - we have intimate knowledge of how they are
            // implemented and that knowledge is relied upon in an unhealthy way; this
            // should be addressed at some stage.

            // load file to memory via resource provider
            var texFile = new RawDataContainer();
            Base.System.GetSingleton().GetResourceProvider()
                .LoadRawDataContainer(filename, texFile, resourceGroup);

			//// get and check existence of CEGUI::System (needed to access ImageCodec)
			//System* sys = System::getSingletonPtr();
			//if (!sys)
			//    CEGUI_THROW(RendererException(
			//        "CEGUI::System object has not been created: "
			//        "unable to access ImageCodec."));

			//Texture* res = sys->getImageCodec().load(texFile, this);

			using (var ms = new MemoryStream())
			{
				texFile.Stream().CopyTo(ms);
				var bytes = ms.ToArray();
				var result = StbImageSharp.ImageResult.FromMemory(bytes, StbImageSharp.ColorComponents.RedGreenBlueAlpha);
				var width = result.Width;
				var height = result.Height;
				var rawBuf = result.Data;


				//var img = FreeImageAPI.FreeImage.LoadFromStream(texFile.Stream());
				//var newImg = FreeImageAPI.FreeImage.ConvertTo32Bits(img);
				//FreeImageAPI.FreeImage.UnloadEx(ref img);

				//var pitch = (int) FreeImageAPI.FreeImage.GetPitch(newImg);
				//var width = (int) FreeImageAPI.FreeImage.GetWidth(newImg);
				//var height = (int) FreeImageAPI.FreeImage.GetHeight(newImg);
				//var redMask = FreeImageAPI.FreeImage.GetRedMask(newImg);
				//var greenMask = FreeImageAPI.FreeImage.GetGreenMask(newImg);
				//var blueMask = FreeImageAPI.FreeImage.GetBlueMask(newImg);

				//var rawBuf = new byte[width*height << 2];
				//FreeImageAPI.FreeImage.ConvertToRawBits(rawBuf, newImg, pitch, 32, redMask, greenMask, blueMask, true);
				//FreeImageAPI.FreeImage.UnloadEx(ref newImg);

				//if (FreeImageAPI.FreeImage.IsLittleEndian())
				//{
				//	var offset = 0;
				//	for (uint i = 0; i < height; ++i)
				//	{
				//		for (uint j = 0; j < width; ++j)
				//		{
				//			var b = rawBuf[offset + 2];
				//			rawBuf[offset + 2] = rawBuf[offset + 0];
				//			rawBuf[offset + 0] = b;
				//			offset += 4;
				//		}
				//	}
				//}

				LoadFromMemory(rawBuf, new Sizef(width, height), PixelFormat.RGBA);
			}

			// unload file data buffer
            Base.System.GetSingleton().GetResourceProvider().UnloadRawDataContainer(texFile);

            //if (!res)
            //    // It's an error
            //    CEGUI_THROW(RendererException(
            //        sys->getImageCodec().getIdentifierString() +
            //        " failed to load image '" + filename + "'."));
        }

        public override void LoadFromMemory(byte[] buffer, Sizef bufferSize, PixelFormat pixelFormat)
        {
            if (!IsPixelFormatSupported(pixelFormat))
                throw new InvalidRequestException("Data was supplied in an unsupported pixel format.");

            InitInternalPixelFormatFields(pixelFormat);
            SetTextureSizeImpl(bufferSize);

            // store size of original data we are loading
            d_dataSize = bufferSize;
            UpdateCachedScaleValues();

            BlitFromMemory(buffer, new Rectf(Vector2.Zero, bufferSize));
        }

        public override void BlitFromMemory(byte[] sourceData, Rectf area)
        {
            // save old texture binding
            int oldTex;
            OpenGL.GL.GetInteger(OpenGL.GetPName.TextureBinding2D, out oldTex);

            // do the real work of getting the data into the texture
            OpenGL.GL.BindTexture(OpenGL.TextureTarget.Texture2D, d_ogltexture);

            if (d_isCompressed)
                LoadCompressedTextureBuffer(area, sourceData);
            else
                LoadUncompressedTextureBuffer(area, sourceData);

            // restore previous texture binding.
            OpenGL.GL.BindTexture(OpenGL.TextureTarget.Texture2D, oldTex);
        }

        public override void BlitFromMemoryARGB(byte[] sourceData, Rectf area)
        {
            BlitFromMemory(sourceData, area);
        }


        public override void BlitFromMemory(IntPtr sourcePtr, Rectf area)
        {
            // save existing config
            int oldTex;
            OpenGL.GL.GetInteger(OpenGL.GetPName.TextureBinding2D, out oldTex);
            OpenGL.GL.BindTexture(OpenGL.TextureTarget.Texture2D, d_ogltexture);

            if (d_isCompressed)
            {
                OpenGL.GL.GetCompressedTexImageInternal(OpenGL.TextureTarget.Texture2D, 0, sourcePtr);
            }
            else
            {
                int oldPack;
                OpenGL.GL.GetInteger(OpenGL.GetPName.PackAlignment, out oldPack);

                OpenGL.GL.PixelStore(OpenGL.PixelStoreParameter.PackAlignment, 1);
                OpenGL.GL.GetTexImageInternal(OpenGL.TextureTarget.Texture2D, 0, d_format, d_subpixelFormat, sourcePtr);

                OpenGL.GL.PixelStore(OpenGL.PixelStoreParameter.PackAlignment, oldPack);
            }

            // restore previous config.
            OpenGL.GL.BindTexture(OpenGL.TextureTarget.Texture2D, oldTex);
        }

        public override bool IsPixelFormatSupported(PixelFormat fmt)
        {
            switch (fmt)
            {
                case PixelFormat.RGBA:
                case PixelFormat.RGB:
                case PixelFormat.RGBA_4444:
                case PixelFormat.RGB_565:
                    return true;

                case PixelFormat.RGB_DXT1:
                case PixelFormat.RGBA_DXT1:
                case PixelFormat.RGBA_DXT3:
                case PixelFormat.RGBA_DXT5:
                    return _owner.IsS3TCSupported();

                default:
                    return false;
            }
        }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        internal OpenGLTexture(OpenGLRendererBase owner, string name)
        {
            d_size = Sizef.Zero;
            d_grabBuffer = null;
            d_dataSize = Sizef.Zero;
            d_texelScaling = Vector2.Zero;
            _owner = owner;
            d_name = name;
        }

        /// <summary>
        /// initliase method that creates a Texture.
        /// </summary>
        protected internal void Initialise()
        {
            InitInternalPixelFormatFields(PixelFormat.RGBA);
            GenerateOpenGLTexture();
        }

        /// <summary>
        /// initliase method that creates a Texture from an image file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="resourceGroup"></param>
        protected internal void Initialise(string filename, string resourceGroup)
        {
            InitInternalPixelFormatFields(PixelFormat.RGBA);
            GenerateOpenGLTexture();
            LoadFromFile(filename, resourceGroup);
        }

        /// <summary>
        /// initialise method that creates a Texture with a given size.
        /// </summary>
        /// <param name="size"></param>
        protected internal void Initialise(Sizef size)
        {
            InitInternalPixelFormatFields(PixelFormat.RGBA);
            GenerateOpenGLTexture();
            SetTextureSize(size);
        }

        /// <summary>
        /// Constructor that wraps an existing GL texture.
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="size"></param>
        protected internal void Initialise(int /*GLuint*/ tex, Sizef size)
        {
            d_ogltexture = tex;
            d_size = size;
            d_dataSize = size;
            InitInternalPixelFormatFields(PixelFormat.RGBA);
            UpdateCachedScaleValues();
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            CleanupOpenGLTexture();
        }

        #endregion

        /// <summary>
        /// generate the OpenGL texture and set some initial options.
        /// </summary>
        protected void GenerateOpenGLTexture()
        {
            // save old texture binding
            int oldTexture;
            OpenGL.GL.GetInteger(OpenGL.GetPName.TextureBinding2D, out oldTexture);
            OpenGL.GL.GenTextures(1, out d_ogltexture);

            // set some parameters for this texture.
            OpenGL.GL.BindTexture(OpenGL.TextureTarget.Texture2D, d_ogltexture);

            OpenGL.GL.TexParameter(OpenGL.TextureTarget.Texture2D,
                                   OpenGL.TextureParameterName.TextureMagFilter,
                                   (int) OpenGL.TextureMagFilter.Linear);
            OpenGL.GL.TexParameter(OpenGL.TextureTarget.Texture2D,
                                   OpenGL.TextureParameterName.TextureMinFilter,
                                   (int) OpenGL.TextureMinFilter.Linear);
            OpenGL.GL.TexParameter(OpenGL.TextureTarget.Texture2D,
                                   OpenGL.TextureParameterName.TextureWrapS,
                                   (int) OpenGL.TextureWrapMode.ClampToEdge);
            OpenGL.GL.TexParameter(OpenGL.TextureTarget.Texture2D,
                                   OpenGL.TextureParameterName.TextureWrapT,
                                   (int) OpenGL.TextureWrapMode.ClampToEdge);

            SetTextureEnvironment();

            // restore previous texture binding.
            OpenGL.GL.BindTexture(OpenGL.TextureTarget.Texture2D, oldTexture);
        }

        /// <summary>
        /// updates cached scale value used to map pixels to texture co-ords.
        /// </summary>
        protected void UpdateCachedScaleValues()
        {
            // Update the scale of a texel based on the absolute size
            d_texelScaling.X = (d_size.Width != 0.0f) ? (1.0f / d_size.Width) : 0.0f;
            d_texelScaling.Y = (d_size.Height != 0.0f) ? (1.0f / d_size.Height) : 0.0f;
        }

        /// <summary>
        /// clean up the GL texture, or the grab buffer if it had been grabbed
        /// </summary>
        protected void CleanupOpenGLTexture()
        {
            // if the grabbuffer is not empty then free it
            if (d_grabBuffer != null)
            {
                //delete[] d_grabBuffer;
                d_grabBuffer = null;
            }
            else
            {
                // otherwise delete any OpenGL texture associated with this object.

                OpenGL.GL.DeleteTextures(1, ref d_ogltexture);
                d_ogltexture = 0;
            }
        }

        /// <summary>
        /// initialise the internal format flags for the given CEGUI::PixelFormat.
        /// </summary>
        /// <param name="fmt"></param>
        protected abstract void InitInternalPixelFormatFields(PixelFormat fmt);

        /// <summary>
        /// internal texture resize function (does not reset format or other fields)
        /// </summary>
        /// <param name="sz"></param>
        protected virtual void SetTextureSizeImpl(Sizef sz)
        {
	        var size = _owner.GetAdjustedTextureSize(sz);
	        d_size = size;

	        // make sure size is within boundaries
	        float maxSize;
	        OpenGL.GL.GetFloat(OpenGL.GetPName.MaxTextureSize, out maxSize);
	        if ((size.Width > maxSize) || (size.Height > maxSize))
		        throw new InvalidOperationException("size too big");

	        // save old texture binding
	        int old_tex;
	        OpenGL.GL.GetInteger(OpenGL.GetPName.TextureBinding2D, out old_tex);

	        // set texture to required size
	        OpenGL.GL.BindTexture(OpenGL.TextureTarget.Texture2D, d_ogltexture);

	        if (d_isCompressed)
	        {
		        var image_size = GetCompressedTextureSize(size);
		        OpenGL.GL.CompressedTexImage2D(OpenGL.TextureTarget.Texture2D,
		                                       0,
		                                       (Lunatics.SDLGL.OpenGL.PixelInternalFormat) d_format,
		                                       (int) (size.Width),
		                                       (int) (size.Height),
		                                       0,
		                                       image_size,
		                                       IntPtr.Zero);
	        }
	        else
	        {
		        OpenGL.GL.TexImage2D(OpenGL.TextureTarget.Texture2D,
		                             0,
		                             (Lunatics.SDLGL.OpenGL.PixelInternalFormat) d_format,
		                             (int) (size.Width),
		                             (int) (size.Height),
		                             0,
		                             d_format,
		                             d_subpixelFormat,
		                             IntPtr.Zero);
	        }

	        // restore previous texture binding.
	        OpenGL.GL.BindTexture(OpenGL.TextureTarget.Texture2D, old_tex);
        }

        protected void LoadUncompressedTextureBuffer(Rectf destArea, byte[] buffer)
        {
	        int oldPack;
	        OpenGL.GL.GetInteger(OpenGL.GetPName.UnpackAlignment, out oldPack);
	        OpenGL.GL.PixelStore(OpenGL.PixelStoreParameter.UnpackAlignment, 1);

	        var ptr = GCHandle.Alloc(buffer, GCHandleType.Pinned);
	        try
	        {
		        OpenGL.GL.TexSubImage2D(OpenGL.TextureTarget.Texture2D,
		                                0,
		                                (int) (destArea.Left),
		                                (int) (destArea.Top),
		                                (int) (destArea.Width),
		                                (int) (destArea.Height),
		                                d_format,
		                                d_subpixelFormat,
		                                ptr.AddrOfPinnedObject());
	        }
	        finally
	        {
		        ptr.Free();
	        }

	        OpenGL.GL.PixelStore(OpenGL.PixelStoreParameter.UnpackAlignment, oldPack);
        }

        protected void LoadCompressedTextureBuffer(Rectf destArea, byte[] buffer)
        {
            var imageSize = GetCompressedTextureSize(destArea.Size);

            var ptr = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                OpenGL.GL.CompressedTexSubImage2D(OpenGL.TextureTarget.Texture2D,
                                           0,
                                           (int)(destArea.Left),
                                           (int)(destArea.Top),
                                           (int)(destArea.Width),
                                           (int)(destArea.Height),
                                           (OpenGL.PixelInternalFormat)d_format,
                                           imageSize,
                                           ptr.AddrOfPinnedObject());
            }
            finally{
                ptr.Free();
            }
        }

        protected virtual int/*GLsizei*/ GetCompressedTextureSize(Sizef pixelSize)
        {
            const int blocksize = 16;
            return (int) (Math.Ceiling(pixelSize.Width/4f)*Math.Ceiling(pixelSize.Height/4f)*blocksize);
        }

        /// <summary>
        /// OpenGL method to set glTexEnv which is deprecated in GL 3.2 and GLES 2.0 and above
        /// </summary>
        protected virtual void SetTextureEnvironment()
        {
            
        }

        #region Fields

        /// <summary>
        /// The OpenGL texture we're wrapping.
        /// </summary>
        protected int d_ogltexture;

        /// <summary>
        /// Size of the texture.
        /// </summary>
        protected Sizef d_size;

        /// <summary>
        /// cached image data for restoring the texture.
        /// </summary>
        private byte[] d_grabBuffer;

        /// <summary>
        /// original pixel of size data loaded into texture
        /// </summary>
        private Sizef d_dataSize;

        /// <summary>
        /// cached pixel to texel mapping scale values.
        /// </summary>
        private Vector2 d_texelScaling;

        /// <summary>
        /// OpenGLRenderer that created and owns this OpenGLTexture
        /// </summary>
        protected OpenGLRendererBase _owner;

        /// <summary>
        /// The name given for this texture.
        /// </summary>
        private string d_name;

        /// <summary>
        /// Texture format
        /// </summary>
        protected OpenGL.PixelFormat d_format;

        /// <summary>
        /// Texture subpixel format
        /// </summary>
        protected OpenGL.PixelType d_subpixelFormat;

        /// <summary>
        /// Whether Texture format is a compressed format
        /// </summary>
        protected bool d_isCompressed;

        #endregion
    }
}

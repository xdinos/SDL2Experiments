using System;
using System.Runtime.InteropServices;
using Lunatics.SDLGL;
using SharpCEGui.Base;

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
    public class OpenGL1Texture : OpenGLTexture
    {
        public OpenGL1Texture(OpenGLRendererBase owner, string name)
                : base(owner, name)
        {
        }

        //! Destructor.
        // TODO: virtual ~OpenGL1Texture();

        public override void BlitToMemory(byte[] targetData)
        {
            // save existing config
            int oldTex;
			
            OpenGL.GL.GetInteger(OpenGL.GetPName.TextureBinding2D, out oldTex);
            OpenGL.GL.BindTexture(OpenGL.TextureTarget.Texture2D, d_ogltexture);

            if (d_isCompressed)
            {
                    OpenGL.GL.GetCompressedTexImage(OpenGL.TextureTarget.Texture2D, 0, targetData);
            }
            else
            {
                int oldPack;
                OpenGL.GL.GetInteger(OpenGL.GetPName.PackAlignment, out oldPack);

                OpenGL.GL.PixelStore(OpenGL.PixelStoreParameter.PackAlignment, 1);
                OpenGL.GL.GetTexImage(OpenGL.TextureTarget.Texture2D, 0, d_format, d_subpixelFormat, targetData);

                OpenGL.GL.PixelStore(OpenGL.PixelStoreParameter.PackAlignment, oldPack);
            }

            // restore previous config.
            OpenGL.GL.BindTexture(OpenGL.TextureTarget.Texture2D, oldTex);
        }

        /// <summary>
        /// OpenGL method to set glTexEnv which is deprecated in GL 3.2 and GLES 2.0 and above
        /// </summary>
        protected override void SetTextureEnvironment()
        {
	        OpenGL.GL.TexEnv(TextureEnvTarget.TextureEnv,
	                         TextureEnvParameter.TextureEnvMode,
	                         (int) /*All.Modulate*/0x2100);
        }

        /// <summary>
        /// initialise the internal format flags for the given CEGUI::PixelFormat.
        /// </summary>
        /// <param name="fmt"></param>
        protected override void InitInternalPixelFormatFields(PixelFormat fmt)
        {
            d_isCompressed = false;

            switch (fmt)
            {
                case PixelFormat.RGBA:
                    d_format = OpenGL.PixelFormat.Rgba;
                    d_subpixelFormat = OpenGL.PixelType.UnsignedByte;
                    break;

                case PixelFormat.RGB:
                    d_format = OpenGL.PixelFormat.Rgb;
                    d_subpixelFormat = OpenGL.PixelType.UnsignedByte;
                    break;

                case PixelFormat.RGB_565:
                    d_format = OpenGL.PixelFormat.Rgb;
                    d_subpixelFormat = OpenGL.PixelType.UnsignedShort565;
                    break;

                case PixelFormat.RGBA_4444:
                    d_format = OpenGL.PixelFormat.Rgb;
                    d_subpixelFormat = OpenGL.PixelType.UnsignedShort4444;
                    break;

                case PixelFormat.RGB_DXT1:
                    //GL_COMPRESSED_RGB_S3TC_DXT1_EXT = ((int)0x83F0);
                    d_format = (OpenGL.PixelFormat) 0x83F0;
                    d_subpixelFormat = OpenGL.PixelType.UnsignedByte; // not used.
                    d_isCompressed = true;
                    break;

                case PixelFormat.RGBA_DXT1:
                    //GL_COMPRESSED_RGBA_S3TC_DXT1_EXT = ((int)0x83F1);
                    d_format = (OpenGL.PixelFormat) 0x83F1;
                    d_subpixelFormat = OpenGL.PixelType.UnsignedByte; // not used.
                    d_isCompressed = true;
                    break;

                case PixelFormat.RGBA_DXT3:
                    //GL_COMPRESSED_RGBA_S3TC_DXT3_EXT = ((int)0x83F2);
                    d_format = (OpenGL.PixelFormat) 0x83F2;
                    d_subpixelFormat = OpenGL.PixelType.UnsignedByte; // not used.
                    d_isCompressed = true;
                    break;

                case PixelFormat.RGBA_DXT5:
                    //GL_COMPRESSED_RGBA_S3TC_DXT5_EXT = ((int)0x83F3);
                    d_format = (OpenGL.PixelFormat) 0x83F3;
                    d_subpixelFormat = OpenGL.PixelType.UnsignedByte; // not used.
                    d_isCompressed = true;
                    break;

                default:
                    throw new InvalidOperationException("invalid or unsupported CEGUI::PixelFormat.");
            }
        }

        protected override void SetTextureSizeImpl(Sizef sz)
        {
            var size = _owner.GetAdjustedTextureSize(sz);
            d_size = size;

            // make sure size is within boundaries
            float/*GLfloat*/ maxSize;

            OpenGL.GL.GetFloat(OpenGL.GetPName.MaxTextureSize, out maxSize);

            if ((size.Width > maxSize) || (size.Height > maxSize))
                throw new InvalidOperationException("size too big");

            // save old texture binding
            int/*GLuint*/ oldTex;
            OpenGL.GL.GetInteger(OpenGL.GetPName.TextureBinding2D, out oldTex);

            // set texture to required size
            OpenGL.GL.BindTexture(OpenGL.TextureTarget.Texture2D, d_ogltexture);
            
            if(d_isCompressed)
            {
                var imageSize = GetCompressedTextureSize(size);
                OpenGL.GL.CompressedTexImage2D(OpenGL.TextureTarget.Texture2D,
                                               0,
                                               (OpenGL.PixelInternalFormat) d_format,
                                               (int) size.Width,
                                               (int) d_size.Height,
                                               0,
                                               imageSize,
                                               IntPtr.Zero);
            }
            else
            {
                OpenGL.GL.TexImage2D(OpenGL.TextureTarget.Texture2D,
                                     0,
                                     OpenGL.PixelInternalFormat.Rgba8,
                                     (int) size.Width,
                                     (int) d_size.Height,
                                     0,
                                     OpenGL.PixelFormat.Rgba,
                                     OpenGL.PixelType.UnsignedByte,
                              IntPtr.Zero);;
            }

            // restore previous texture binding.
            OpenGL.GL.BindTexture(OpenGL.TextureTarget.Texture2D, oldTex);
        }

        protected override int GetCompressedTextureSize(Sizef pixelSize)
        {
            var blocksize = 16;
            if (d_format == (OpenGL.PixelFormat) 0x83F0 /*GL_COMPRESSED_RGB_S3TC_DXT1_EXT*/||
                d_format == (OpenGL.PixelFormat) 0x83F1 /*GL_COMPRESSED_RGBA_S3TC_DXT1_EXT*/)
                blocksize = 8;

            return (int) (Math.Ceiling(pixelSize.Width/4)*Math.Ceiling(pixelSize.Height/4)*blocksize);
        }
    }
}
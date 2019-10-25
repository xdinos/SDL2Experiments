#region Copyright
// Copyright (C) 2004 - 2013 Paul D Turner & The CEGUI Development Team
// 
// C# Port developed by The SharpCEGui Development Team
// Copyright (C) 2012 - 2013
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Abstract base class specifying the required interface for Texture objects.
    /// <para>
    /// Texture objects are created via the Renderer.  The actual inner workings of
    /// any Texture object are dependant upon the Renderer (and underlying API) in
    /// use.  This base class defines the minimal set of functions that is required
    /// for the rest of the system to work.  Texture objects are only created
    /// through the Renderer object's texture creation functions.
    /// </para>
    /// </summary>
    public abstract class Texture
    {
        /// <summary>
        /// Enumerated type containing the supported pixel formats that can be
        /// passed to loadFromMemory
        /// </summary>
        public enum PixelFormat
        {
            /// <summary>
            /// Each pixel is 3 bytes. RGB in that order.
            /// </summary>
            RGB,

            /// <summary>
            /// Each pixel is 4 bytes. RGBA in that order.
            /// </summary>
            RGBA,

            /// <summary>
            /// Each pixel is 2 bytes. RGBA in that order.
            /// </summary>
            RGBA_4444,

            /// <summary>
            /// Each pixel is 2 bytes. RGB in that order.
            /// </summary>
            RGB_565,

            /// <summary>
            /// PVR texture compression. Each pixel is 2 bits.
            /// </summary>
            PVRTC2,

            /// <summary>
            /// PVR texture compression. Each pixel is 4 bits.
            /// </summary>
            PVRTC4,

            /// <summary>
            /// S3 DXT1 texture compression (RGB).
            /// </summary>
            RGB_DXT1,

            /// <summary>
            /// S3 DXT1 texture compression (RGBA).
            /// </summary>
            RGBA_DXT1,

            /// <summary>
            /// S3 DXT1 texture compression (RGBA).
            /// </summary>
            RGBA_DXT3,

            /// <summary>
            /// S3 DXT1 texture compression (RGBA).
            /// </summary>
            RGBA_DXT5
        }
        
        /// <summary>
        /// Returns the name given to the texture when it was created.
        /// </summary>
        /// <returns>
        /// Reference to a String object that holds the name of the texture.
        /// </returns>
        public abstract string GetName();

        /// <summary>
        /// Returns the current pixel size of the texture.
        /// </summary>
        /// <returns>
        /// Reference to a Size object that describes the size of the texture in
        /// pixels.
        /// </returns>
        public abstract Sizef GetSize();

        /// <summary>
        /// Returns the original pixel size of the data loaded into the texture.
        /// </summary>
        /// <returns>
        /// reference to a Size object that describes the original size, in pixels,
        /// of the data loaded into the texture.
        /// </returns>
        public abstract Sizef GetOriginalDataSize();

        /// <summary>
        /// Returns pixel to texel scale values that should be used for converting
        /// pixel values to texture co-ords.
        /// </summary>
        /// <returns>
        /// Reference to a Vector2 object that describes the scaling values required
        /// to accurately map pixel positions to texture co-ordinates.
        /// </returns>
        public abstract Lunatics.Mathematics.Vector2 GetTexelScaling();

        /// <summary>
        /// Loads the specified image file into the texture.  The texture is resized
        /// as required to hold the image.
        /// </summary>
        /// <param name="filename">
        /// The filename of the image file that is to be loaded into the texture
        /// </param>
        /// <param name="resourceGroup">
        /// Resource group identifier to be passed to the resource provider when
        /// loading the image file.
        /// </param>
        public abstract void LoadFromFile(string filename, string resourceGroup);

        /// <summary>
        /// Loads (copies) an image in memory into the texture.  The texture is
        /// resized as required to hold the image.
        /// </summary>
        /// <param name="buffer">
        /// Pointer to the buffer containing the image data.
        /// </param>
        /// <param name="bufferSize">
        /// Size of the buffer (in pixels as specified by \a pixelFormat)
        /// </param>
        /// <param name="pixelFormat">
        /// PixelFormat value describing the format contained in \a buffPtr.
        /// </param>
        public abstract void LoadFromMemory(byte[] buffer, Sizef bufferSize, PixelFormat pixelFormat);

        /// <summary>
        /// Performs an area memory blit to the texture
        /// </summary>
        /// <param name="sourceData">
        /// input data, the size must match area described by the given Rect
        /// </param>
        /// <param name="area">
        /// area where the blit will happen
        /// </param>
        /// <remarks>
        /// The pixel format must match current Texture's pixel format!
        /// </remarks>
        public abstract void BlitFromMemory(byte[] sourceData, Rectf area);
        public abstract void BlitFromMemory(IntPtr sourcePtr, Rectf area);

        public abstract void BlitFromMemoryARGB(byte[] sourceData, Rectf area);
        //public abstract void BlitFromMemoryARGB(IntPtr sourcePtr, Rectf area);

        /// <summary>
        /// Performs a complete blit from the texture surface to memory
        /// </summary>
        /// <param name="targetData">
        /// targetData the buffer where the target is stored
        /// </param>
        /// <remarks>
        /// You have to (correctly) preallocate the target buffer!
        /// </remarks>
        public abstract void BlitToMemory(byte[] targetData);

        /// <summary>
        /// Return whether the specified pixel format is supported by the system for
        /// the CEGUI::Texture implementation.
        /// <para>
        /// The result of this call will vary according to the implementaion API
        /// and the capabilities of the hardware.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Whether the CEGUI system as a whole will make use of support for any
        /// given pixel format will depend upon that format being recognised and
        /// supported by both the renderer module implementation and the ImageCodec
        /// module that is used to load texture data.
        /// </remarks>
        /// <param name="fmt">
        /// One of the PixelFormat enumerated values specifying the pixel format that is to be tested.
        /// </param>
        /// <returns>
        /// - true if the specified PixelFormat is supported.
        /// - false if the specified PixelFormat is not supported.
        /// </returns>
        public abstract bool IsPixelFormatSupported(PixelFormat fmt);

    }
}
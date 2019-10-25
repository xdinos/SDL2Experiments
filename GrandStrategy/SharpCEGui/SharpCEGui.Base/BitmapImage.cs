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

using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class BitmapImage : Image
    {
        public const string TypeName = "BitmapImage";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public BitmapImage(string name) 
            : base(name)
        {
            _texture = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="attributes"></param>
        public BitmapImage(XMLAttributes attributes)
                : this(attributes.GetValueAsString(ImageNameAttribute),
                       System.GetSingleton().GetRenderer()
                             .GetTexture(attributes.GetValueAsString(ImageTextureAttribute)),
                       new Rectf(new Lunatics.Mathematics.Vector2(attributes.GetValueAsInteger(ImageXPosAttribute),
                                                                 attributes.GetValueAsInteger(ImageYPosAttribute)),
                                 new Sizef(attributes.GetValueAsInteger(ImageWidthAttribute),
                                           attributes.GetValueAsInteger(ImageHeightAttribute))),
                       new Lunatics.Mathematics.Vector2(attributes.GetValueAsInteger(ImageXOffsetAttribute),
                                                       attributes.GetValueAsInteger(ImageYOffsetAttribute)),
                       PropertyHelper.FromString<AutoScaledMode>(attributes.GetValueAsString(ImageAutoScaledAttribute)),
                       new Sizef(attributes.GetValueAsInteger(ImageNativeHorzResAttribute, 640),
                                 attributes.GetValueAsInteger(ImageNativeVertResAttribute, 480)))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="texture"></param>
        /// <param name="pixelArea"></param>
        /// <param name="pixelOffset"></param>
        /// <param name="autoScaled"></param>
        /// <param name="nativeResolution"></param>
        public BitmapImage(string name, Texture texture,
                           Rectf pixelArea, Lunatics.Mathematics.Vector2 pixelOffset,
                           AutoScaledMode autoScaled, Sizef nativeResolution)
                : base(name, pixelOffset, pixelArea, autoScaled, nativeResolution)
        {
            _texture = texture;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture"></param>
        public void SetTexture(Texture texture)
        {
            _texture = texture;
        }

        public override List<GeometryBuffer> CreateRenderGeometry(ImageRenderSettings renderSettings)
        {
            Rectf texRect = Rectf.Zero, finalRect;

            var isFullClipped = CalculateTextureAndRenderAreas(renderSettings.DestArea, ref renderSettings.ClipArea, out finalRect, ref texRect);
            if (isFullClipped)
                return new List<GeometryBuffer>();

            var vbuffer = new TexturedColouredVertex[6];
            var colours = renderSettings.MultiplyColours;

            CreateTexturedQuadVertices(vbuffer, colours, ref finalRect, ref texRect);
            
            #region original code
            var buffer = System.GetSingleton().GetRenderer().CreateGeometryBufferTextured();

            buffer.SetClippingActive(renderSettings.IsClippingEnabled);
            if (renderSettings.IsClippingEnabled && renderSettings.ClipArea.HasValue)
                buffer.SetClippingRegion(renderSettings.ClipArea.Value);
            buffer.SetTexture("texture0", _texture);
            buffer.AppendGeometry(vbuffer);
            buffer.SetAlpha(renderSettings.Alpha);

            var geomBuffers = new List<GeometryBuffer> {buffer};
            return geomBuffers;

            #endregion

            #region xdinos new code

            //GeometryBuffer buffer = imageGeometryBuffers.Count > 0
            //                                ? imageGeometryBuffers[imageGeometryBuffers.Count - 1]
            //                                : null;
            //if (buffer != null)
            //{
            //    if (_lastTexture != _texture ||
            //        (_lastClippingEnabled.HasValue && _lastClippingEnabled.Value != renderSettings.clippingEnabled) ||
            //        (_lastClipArea != renderSettings.clipArea) ||
            //        (_lastAlpha.HasValue && _lastAlpha.Value != renderSettings.alpha))
            //        buffer = null;
            //}

            //if (buffer == null)
            //{
            //    buffer = System.GetSingleton().GetRenderer().CreateGeometryBufferTextured();
            //    imageGeometryBuffers.Add(buffer);

            //    buffer.SetClippingActive(renderSettings.clippingEnabled);
            //    if (renderSettings.clippingEnabled && renderSettings.clipArea.HasValue)
            //        buffer.SetClippingRegion(renderSettings.clipArea.Value);
            //    buffer.SetTexture("texture0", _texture);
            //    buffer.SetAlpha(renderSettings.alpha);

            //    _lastClippingEnabled = renderSettings.clippingEnabled;
            //    _lastClipArea = renderSettings.clipArea;
            //    _lastAlpha = renderSettings.alpha;
            //    _lastTexture = _texture;
            //}

            //buffer.AppendGeometry(vbuffer);

            #endregion
        }

        public override void AddToRenderGeometry(GeometryBuffer geomBuffer, ref Rectf renderArea, ref Rectf? clipArea, ColourRect colours)
        {
            Rectf texRect = Rectf.Zero, finalRect;
            var isFullClipped = CalculateTextureAndRenderAreas(renderArea, ref clipArea, out finalRect, ref texRect);
            if (isFullClipped)
                return;
            var vbuffer = new TexturedColouredVertex[6];
            CreateTexturedQuadVertices(vbuffer, colours, ref finalRect, ref texRect);
            geomBuffer.AppendGeometry(vbuffer);
        }

        private bool CalculateTextureAndRenderAreas(Rectf renderSettingDestArea, ref Rectf? clippingArea, out Rectf finalRect, ref Rectf texRect)
        {
            var dest = (renderSettingDestArea);
            // apply rendering offset to the destination Rect
            dest.Offset(d_scaledOffset);
            
            // get the rect area that we will actually draw to (i.e. perform clipping)
            finalRect = clippingArea.HasValue ? dest.GetIntersection(clippingArea.Value) : dest;
            
            // check if rect was totally clipped
            if ((finalRect.Width == 0) || (finalRect.Height == 0))
                return true;
            
            // Obtain correct scale values from the texture
            var texelScale = _texture.GetTexelScaling();
            var texPerPix = new Lunatics.Mathematics.Vector2(d_imageArea.Width/dest.Width,
                                                              d_imageArea.Height/dest.Height);
            // calculate final, clipped, texture co-ordinates
            texRect = new Rectf((d_imageArea.d_min + ((finalRect.d_min - dest.d_min)*texPerPix))*texelScale,
                                (d_imageArea.d_max + ((finalRect.d_max - dest.d_max)*texPerPix))*texelScale);
            
            // TODO: This is clearly not optimal but the only way to go with the current
            // Font rendering system. Distance field rendering would allow us to ignore the 
            // pixel alignment.
            finalRect.d_min.X =  CoordConverter.AlignToPixels(finalRect.d_min.X);
            finalRect.d_min.Y =  CoordConverter.AlignToPixels(finalRect.d_min.Y);
            finalRect.d_max.X =  CoordConverter.AlignToPixels(finalRect.d_max.X);
            finalRect.d_max.Y =  CoordConverter.AlignToPixels(finalRect.d_max.Y);
            
            return false;
        }

        private void CreateTexturedQuadVertices(TexturedColouredVertex[] vbuffer, ColourRect colours, ref Rectf finalRect, ref Rectf texRect)
        {
            // vertex 0
            vbuffer[0].Position = new Lunatics.Mathematics.Vector3(finalRect.Left, finalRect.Top, 0.0f);
            vbuffer[0].SetColour(colours.d_top_left);
            vbuffer[0].TextureCoordinates = new Lunatics.Mathematics.Vector2(texRect.Left, texRect.Top);

            // vertex 1
            vbuffer[1].Position = new Lunatics.Mathematics.Vector3(finalRect.Left, finalRect.Bottom, 0.0f);
            vbuffer[1].SetColour(colours.d_bottom_left);
            vbuffer[1].TextureCoordinates = new Lunatics.Mathematics.Vector2(texRect.Left, texRect.Bottom);

            // vertex 2
            vbuffer[2].Position.X = finalRect.Right;
            vbuffer[2].Position.Z = 0.0f;
            vbuffer[2].SetColour(colours.d_bottom_right);
            vbuffer[2].TextureCoordinates.X = texRect.Right;

            // Quad splitting done from top-left to bottom-right diagonal
            vbuffer[2].Position.Y = finalRect.Bottom;
            vbuffer[2].TextureCoordinates.Y = texRect.Bottom;

            // vertex 3
            vbuffer[3].Position = new Lunatics.Mathematics.Vector3(finalRect.Right, finalRect.Top, 0.0f);
            vbuffer[3].SetColour(colours.d_top_right);
            vbuffer[3].TextureCoordinates = new Lunatics.Mathematics.Vector2(texRect.Right, texRect.Top);

            // vertex 4
            vbuffer[4].Position.X = finalRect.Left;
            vbuffer[4].Position.Z = 0.0f;
            vbuffer[4].SetColour(colours.d_top_left);
            vbuffer[4].TextureCoordinates.X = texRect.Left;

            // Quad splitting done from top-left to bottom-right diagonal
            vbuffer[4].Position.Y = finalRect.Top;
            vbuffer[4].TextureCoordinates.Y = texRect.Top;

            // vertex 5
            vbuffer[5].Position = new Lunatics.Mathematics.Vector3(finalRect.Right, finalRect.Bottom, 0.0f);
            vbuffer[5].SetColour(colours.d_bottom_right);
            vbuffer[5].TextureCoordinates = new Lunatics.Mathematics.Vector2(texRect.Right, texRect.Bottom);
        }

        #region xdinos new code
        //private static bool? _lastClippingEnabled = null;
        //private static Rectf? _lastClipArea = null;
        //private static float? _lastAlpha = null;
        //private static Texture _lastTexture = null;
        #endregion

        #region Fields

        /// <summary>
        /// Texture used by this image.
        /// </summary>
        private Texture _texture;

        #endregion

        #region Constants

        private const string ImageTypeAttribute = "type";
        private const string ImageNameAttribute = "name";
        private const string ImageTextureAttribute = "texture";
        private const string ImageXPosAttribute = "xPos";
        private const string ImageYPosAttribute = "yPos";
        private const string ImageWidthAttribute ="width";
        private const string ImageHeightAttribute = "height";
        private const string ImageXOffsetAttribute = "xOffset";
        private const string ImageYOffsetAttribute = "yOffset";
        private const string ImageAutoScaledAttribute = "autoScaled";
        private const string ImageNativeHorzResAttribute = "nativeHorzRes";
        private const string ImageNativeVertResAttribute = "nativeVertRes";

        #endregion
    }
}
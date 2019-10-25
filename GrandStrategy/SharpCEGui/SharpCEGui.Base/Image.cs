#region Copyright
// Copyright (C) 2004 - 2013 Paul D Turner & The CEGUI Development Team
// 
// C# Port developed by The SharpCEGui Development Team
// Copyright (C) 2012 - 2019
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
using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// A struct that contains the render settings for the Image class.
    /// </summary>
    public /*struct*/ class ImageRenderSettings
    {
        #region Constructors

        public ImageRenderSettings(Rectf destArea)
                : this(destArea, null, false, new ColourRect(0XFFFFFFFF), 1f)
        {
        }

        public ImageRenderSettings(Rectf destArea, Rectf? clipArea, bool isClippingEnabled)
            : this(destArea, clipArea, isClippingEnabled, new ColourRect(0XFFFFFFFF), 1f)
        {
        }

        public ImageRenderSettings(Rectf destArea, Rectf? clipArea, bool isClippingEnabled,
                                   ColourRect multiplicationColours)
            : this(destArea, clipArea, isClippingEnabled, multiplicationColours, 1f)
        {
        }

        public ImageRenderSettings(Rectf destArea, Rectf? clipArea, bool isClippingEnabled,
                                   ColourRect multiplicationColours, float alpha)
        {
            DestArea = destArea;
            ClipArea = clipArea;
            IsClippingEnabled = isClippingEnabled;
            MultiplyColours = multiplicationColours;
            this.Alpha = alpha;
        }

        public ImageRenderSettings(ImageRenderSettings source)
            : this(source.DestArea, source.ClipArea, source.IsClippingEnabled, source.MultiplyColours, source.Alpha)
        {
        }

        #endregion

        #region Fields

        /// <summary>
        /// The destination area for the Image.
        /// </summary>
        public Rectf DestArea;

        /// <summary>
        /// The clipping area of the Image.
        /// </summary>
        public Rectf? ClipArea;

        /// <summary>
        /// True of clipping should be enabled for the geometry of this Image.
        /// </summary>
        public bool IsClippingEnabled;

        /// <summary>
        /// The colour rectangle set for this Image. The colours of the rectangle will be multiplied with
        /// the Image's colours when rendered, i.e. if the colours are all '0xFFFFFFFF' no effect will be seen.
        /// If this will be used depends on the underlying image.
        /// </summary>
        public ColourRect MultiplyColours;

        /// <summary>
        /// The alpha value for this image, should be set as the GeometryBuffer's
        /// alpha by the underlying image class
        /// </summary>
        public float Alpha;

        #endregion
    }

    /// <summary>
    /// Interface for Image.
    /// <para>
    /// In CEGUI, an Image is some object that can render itself into a given
    /// GeometryBuffer object.  This may be something as simple as a basic textured
    /// quad, or something more complex.
    /// </para>
    /// </summary>
    public abstract class Image : ChainedXmlHandler, IDisposable
    {
        #region Constructors

        protected Image(string name)
            : this(name, Lunatics.Mathematics.Vector2.Zero, Rectf.Zero, AutoScaledMode.Disabled, Sizef.Zero)
        {
        }

        protected Image(string name,
                        Lunatics.Mathematics.Vector2 pixelOffset,
                        Rectf imageArea,
                        AutoScaledMode autoScaled,
                        Sizef nativeResolution)
        {
            d_name = name;
            d_pixelOffset = pixelOffset;
            d_imageArea = imageArea;
            d_autoScaled = autoScaled;
            d_nativeResolution = nativeResolution;
            d_scaledSize = Sizef.Zero;
            d_scaledOffset = Lunatics.Mathematics.Vector2.Zero;

            // force initialisation of the autoscaling fields.
            UpdateScaledSizeAndOffset(System.GetSingleton().GetRenderer().GetDisplaySize());
        }

        #endregion

        #region IDisposable / Destructor

        // TODO: virtual ~Image();

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        #endregion

        /// <summary>
        /// Returns the name of the Image.
        /// </summary>
        /// <returns>
        /// The name of the Image.
        /// </returns>
        public string GetName()
        {
            return d_name;
        }

        /// <summary>
        /// Returns the rendered size of this Image considering the autoscale
        /// options that were set.
        /// </summary>
        /// <returns>
        /// The rendered size of this Image.
        /// </returns>
        public Sizef GetRenderedSize()
        {
            return d_scaledSize;
        }

        /// <summary>
        /// Returns the rendered offset of this Image considering the autoscale
        /// options that were set.
        /// </summary>
        /// <returns>
        /// The rendered offset of this Image.
        /// </returns>
        public Lunatics.Mathematics.Vector2 GetRenderedOffset()
        {
            return d_scaledOffset;
        }

        /// <summary>
        /// Creates a container of GeometryBuffers based on the Image, providing the 
        /// geometry data needed for rendering.
        /// </summary>
        /// <param name="renderSettings">
        /// The ImageRenderSettings that contain render settings for new GeometryBuffers.
        /// </param>
        public abstract List<GeometryBuffer> CreateRenderGeometry(ImageRenderSettings renderSettings);

        /// <summary>
        /// Appends additional render geometry for this image to an GeometryBuffers.
        /// The GeometryBuffer must be created beforehand and must feature render
        /// settings that allow adding this image geometry into the same render batch.
        /// Batching compatibility has to be ensured before this call.
        /// </summary>
        /// <param name="geomBuffer">
        /// The existing GeometryBuffers to which the new render geometry will be appended.
        /// </param>
        /// <param name="renderArea">
        /// The target area at which the image should be rendered.
        /// </param>
        /// <param name="clipAreaColourRect">
        /// </param>
        /// <param name="colours">
        /// Multiplicative colours to be applied to the text.
        /// </param>
        public abstract void AddToRenderGeometry(GeometryBuffer geomBuffer, ref Rectf renderArea, ref Rectf? clipAreaColourRect, ColourRect colours);

        /// <summary>
        /// Notifies the class that the display size of the renderer has changed so that
        /// the window can adapt to the new display size accordingly.
        /// </summary>
        /// <param name="rendererDisplaySize">
        /// The new display size.
        /// </param>
        public virtual void NotifyDisplaySizeChanged(Sizef rendererDisplaySize)
        {
            //If we use autoscaling of any sort we must update the scaled size and offset
            if (d_autoScaled != AutoScaledMode.Disabled)
                UpdateScaledSizeAndOffset(rendererDisplaySize);
        }

        /// <summary>
        /// Sets the rectangular image area of this Image.
        /// </summary>
        /// <param name="imageArea">
        /// The rectangular image area of this Image.
        /// </param>
        public void SetImageArea(Rectf imageArea)
        {
            d_imageArea = imageArea;

            if (d_autoScaled != AutoScaledMode.Disabled)
                UpdateScaledSizeAndOffset(System.GetSingleton().GetRenderer().GetDisplaySize());
            else
                d_scaledSize = d_imageArea.Size;
        }

        /// <summary>
        /// Sets the pixel offset of this Image.
        /// </summary>
        /// <param name="pixelOffset">
        /// The pixel offset of this Image.
        /// </param>
        public void SetOffset(Lunatics.Mathematics.Vector2 pixelOffset)
        {
             d_pixelOffset = pixelOffset;
            
            if (d_autoScaled != AutoScaledMode.Disabled)
                UpdateScaledOffset(System.GetSingleton().GetRenderer().GetDisplaySize());
            else
                d_scaledOffset = d_pixelOffset;
        }

        /// <summary>
        /// Sets the autoscale mode of this Image.
        /// </summary>
        /// <param name="autoScaled">
        /// The  autoscale mode of this Image.
        /// </param>
        public void SetAutoScaled(AutoScaledMode autoScaled)
        {
            d_autoScaled = autoScaled;
            
            if (d_autoScaled != AutoScaledMode.Disabled)
                UpdateScaledSizeAndOffset(System.GetSingleton().GetRenderer().GetDisplaySize());
            else
            {
                d_scaledSize = d_imageArea.Size;
                d_scaledOffset = d_pixelOffset;
            }
        }

        /// <summary>
        /// Sets the autoscale native resolution of this Image.
        /// </summary>
        /// <param name="nativeResolution">
        /// The  autoscale native resolution of this Image.
        /// </param>
        public void SetNativeResolution(Sizef nativeResolution)
        {
            d_nativeResolution = nativeResolution;
            
            if (d_autoScaled != AutoScaledMode.Disabled)
                UpdateScaledSizeAndOffset(System.GetSingleton().GetRenderer().GetDisplaySize());
        }

        #region Standard Image.Render overloads

        //public void Render(List<GeometryBuffer> imageGeometryBuffers, Rectf destArea, Rectf? clipArea, bool clippingEnabled)
        //{
        //    Render(imageGeometryBuffers, destArea, clipArea, clippingEnabled, new ColourRect(0xffffffff));
        //}

        //public void Render(List<GeometryBuffer> imageGeometryBuffers, Rectf destArea, Rectf? clipArea, bool clippingEnabled,
        //                   ColourRect multiplicationColours)
        //{
        //    Render(imageGeometryBuffers, destArea, clipArea, clippingEnabled, multiplicationColours, 1f);
        //}

        //public void Render(List<GeometryBuffer> imageGeometryBuffers, Rectf destArea, Rectf? clipArea, bool clippingEnabled,
        //                   ColourRect multiplicationColours, float alpha)
        //{
        //    var renderSettings = new ImageRenderSettings(destArea, clipArea, clippingEnabled, multiplicationColours, alpha);
        //    Render(imageGeometryBuffers, renderSettings);
        //}

        //public void Render(List<GeometryBuffer> imageGeometryBuffers, Lunatics.Mathematics.Vector2 position)
        //{
        //    Render(imageGeometryBuffers, position, null);
        //}

        //public void Render(List<GeometryBuffer> imageGeometryBuffers, Lunatics.Mathematics.Vector2 position,
        //                   Rectf? clipArea)
        //{
        //    Render(imageGeometryBuffers, position, clipArea, false);
        //}
        //public void Render(List<GeometryBuffer> imageGeometryBuffers, 
        //    Lunatics.Mathematics.Vector2 position,
        //                   Rectf? clipArea, bool clippingEnabled)
        //{
        //    var renderSettings = new ImageRenderSettings(new Rectf(position, GetRenderedSize()), clipArea, clippingEnabled);
        //    Render(imageGeometryBuffers, renderSettings);
        //}

        //public void Render(List<GeometryBuffer> imageGeometryBuffers,
        //                   Lunatics.Mathematics.Vector2 position,
        //                   Rectf? clipArea,
        //                   bool clippingEnabled,
        //                   ColourRect colours,
        //                   float alpha = 1.0f)
        //{
        //    var renderSettings = new ImageRenderSettings(new Rectf(position, GetRenderedSize()), clipArea, clippingEnabled, colours, alpha);
        //    Render(imageGeometryBuffers, renderSettings);
        //}


        //public void Render(List<GeometryBuffer> imageGeometryBuffers,
        //                   Lunatics.Mathematics.Vector2 position,
        //                   Sizef size,
        //                   Rectf? clipArea = null,
        //                   bool clippingEnabled = false)
        //{
        //    var renderSettings = new ImageRenderSettings(new Rectf(position, size), clipArea, clippingEnabled);
        //    Render(imageGeometryBuffers, renderSettings);
        //}

        //public void Render(List<GeometryBuffer> imageGeometryBuffers,
        //                   Lunatics.Mathematics.Vector2 position,
        //                   Sizef size,
        //                   Rectf? clipArea,
        //                   bool clippingEnabled,
        //                   ColourRect colours,
        //                   float alpha = 1.0f)
        //{
        //    var renderSettings = new ImageRenderSettings(new Rectf(position, size), clipArea, clippingEnabled, colours, alpha);
        //    Render(imageGeometryBuffers, renderSettings);
        //}

        #endregion

        /// <summary>
        /// Helper able to compute scaling factors for auto scaling
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="displaySize"></param>
        /// <param name="nativeDisplaySize"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <remarks>
        /// This is mostly for internal use, unless you know what you are doing,
        /// please don't touch this method!
        /// </remarks>
        public static void ComputeScalingFactors(AutoScaledMode mode,
                                                 Sizef displaySize,
                                                 Sizef nativeDisplaySize,
                                                 out float xScale,
                                                 out float yScale)
        {
            if (mode == AutoScaledMode.Disabled)
            {
                xScale = 1.0f;
                yScale = 1.0f;
            }
            else if (mode == AutoScaledMode.Vertical)
            {
                xScale = displaySize.Height/nativeDisplaySize.Height;
                yScale = xScale;
            }
            else if (mode == AutoScaledMode.Horizontal)
            {
                xScale = displaySize.Width/nativeDisplaySize.Width;
                yScale = xScale;
            }
            else if (mode == AutoScaledMode.Min)
            {
                xScale = global::System.Math.Min(displaySize.Width/nativeDisplaySize.Width,
                                                 displaySize.Height/nativeDisplaySize.Height);
                yScale = xScale;
            }
            else if (mode == AutoScaledMode.Max)
            {
                xScale = global::System.Math.Max(displaySize.Width/nativeDisplaySize.Width,
                                                 displaySize.Height/nativeDisplaySize.Height);
                yScale = xScale;
            }
            else if (mode == AutoScaledMode.Both)
            {
                xScale = displaySize.Width/nativeDisplaySize.Width;
                yScale = displaySize.Height/nativeDisplaySize.Height;
            }
            else
            {
                xScale = 0f;
                yScale = 0f;
                global::System.Diagnostics.Debug.Assert(false, "Invalid AutoScaledMode");
            }
        }

        public Lunatics.Mathematics.Vector2 ComputeScalingFactors(AutoScaledMode mode, Sizef displaySize, Sizef nativeDisplaySize)
        {
            float xScale, yScale;
            ComputeScalingFactors(mode, displaySize, nativeDisplaySize, out xScale, out yScale);
            return new Lunatics.Mathematics.Vector2(xScale, yScale);
        }

        protected override void ElementStartLocal(string element, XMLAttributes attributes)
        {
            System.GetSingleton().Logger.LogEvent("    [Image] Unknown XML tag encountered: " + element);
        }

        protected override void ElementEndLocal(string element)
        {
            if (element == "Image")
                d_completed = true;
        }

        /// <summary>
        /// Updates the scaled size and offset values according to the new display size of the renderer 
        /// </summary>
        /// <param name="rendererDisplaySize"></param>
        protected void UpdateScaledSizeAndOffset(Sizef rendererDisplaySize)
        {
            // TODO: ...
            //glm::vec2 scaleFactors;
            //ComputeScalingFactors(d_autoScaled, rendererDisplaySize, d_nativeResolution, scaleFactors.x, scaleFactors.y);
            
            var scaleFactors = ComputeScalingFactors(d_autoScaled, rendererDisplaySize, d_nativeResolution);
            d_scaledSize = new Sizef(d_imageArea.Size.Width*scaleFactors.X,
                                     d_imageArea.Size.Height*scaleFactors.Y);
            d_scaledOffset = d_pixelOffset * scaleFactors;
        }
        
        /// <summary>
        /// Updates only the scaled size values according to the new display size of the renderer 
        /// </summary>
        /// <param name="rendererDisplaySize"></param>
        protected void UpdateScaledSize(Sizef rendererDisplaySize)
        {
            // TODO: ...
            //glm::vec2 scaleFactors;
            //ComputeScalingFactors(d_autoScaled, rendererDisplaySize, d_nativeResolution, scaleFactors.x, scaleFactors.y);

            var scaleFactors = ComputeScalingFactors(d_autoScaled, rendererDisplaySize, d_nativeResolution);
            d_scaledSize = new Sizef(d_imageArea.Size.Width*scaleFactors.X,
                                     d_imageArea.Size.Height*scaleFactors.Y);
        }
        
        /// <summary>
        /// Updates only the scaled offset values according to the new display size of the renderer 
        /// </summary>
        /// <param name="rendererDisplaySize"></param>
        protected void UpdateScaledOffset(Sizef rendererDisplaySize)
        {
            // TODO: ...
            //glm::vec2 scaleFactors;
            //ComputeScalingFactors(d_autoScaled, rendererDisplaySize, d_nativeResolution, scaleFactors.x, scaleFactors.y);

            var scaleFactors = ComputeScalingFactors(d_autoScaled, rendererDisplaySize, d_nativeResolution);
            d_scaledOffset = d_pixelOffset * scaleFactors;
        }

        #region Fields

        //! Name used for the Image as defined during creation.
        protected string d_name;
        
        //! The rectangular area defined for this Image defining position and size of it in relation to
        //! the underlying data of the Image.
        protected Rectf d_imageArea;
        
        //! The pixel offset of the Image. It defines ???
        protected Lunatics.Mathematics.Vector2/*glm::vec2*/ d_pixelOffset;
        
        //! Whether image is auto-scaled or not and how.
        protected AutoScaledMode d_autoScaled;
        
        //! Native resolution used for autoscaling.
        protected Sizef d_nativeResolution;
        
        //! The size in pixels after having autoscaling applied.
        protected Sizef d_scaledSize;
        
        //! The offset in pixels after having autoscaling applied.
        protected /*glm::vec2*/Lunatics.Mathematics.Vector2 d_scaledOffset;

        #endregion
    }
}
using System;
using System.Collections.Generic;
using Lunatics.Mathematics;

namespace SharpCEGui.Base.Svg
{
    /// <summary>
    ///  Defines the SVGImage class, which describes a vector graphics image that can be
    /// rendered by CEGUI.
    /// </summary>
    public class SvgImage : Image
    {
        public const string TypeName = "SVGImage";

        /// <summary>
        /// A struct that contains the render settings for the SVGImage class.
        /// </summary>
        public /*struct*/ class SvgImageRenderSettings : ImageRenderSettings
        {
            //! Constructor
            public SvgImageRenderSettings(ImageRenderSettings imgRenderSettings,
                                          Vector2 scaleFactor,
                                          bool antiAliasing)
                    : base(imgRenderSettings)

            {
                d_scaleFactor = scaleFactor;
                d_antiAliasing = antiAliasing;
            }

            //! The scaling factor of the geometry
            public Vector2 d_scaleFactor;

            //! Create anti-aliasing geometry using alpha-blending for all shapes
            public bool d_antiAliasing;
        }

        public SvgImage(string name) : base(name)
        {
            d_svgData = null;
            d_useGeometryAntialiasing = true;
        }

        public SvgImage(string name, SvgData svgData)
                : base(name,
                       Vector2.Zero,
                       new Rectf(Vector2.Zero, new Sizef(svgData.getWidth(), svgData.getHeight())),
                       AutoScaledMode.Disabled,
                       new Sizef(640f, 480f))
        {
            d_svgData = svgData;
            d_useGeometryAntialiasing = true;
        }

        public SvgImage(XMLAttributes attributes) : base(attributes.GetValueAsString(ImageNameAttribute))
        {
            throw new NotImplementedException();
        }

        // Implement CEGUI::Image interface
        public override List<GeometryBuffer> CreateRenderGeometry(ImageRenderSettings renderSettings)
        {
            var dest = renderSettings.DestArea;
            // apply rendering offset to the destination Rect
            dest.Offset(d_scaledOffset);

            var clipArea = renderSettings.ClipArea;
            // Calculate the actual (clipped) area to which we want to render to
            var finalRect = clipArea.HasValue ? dest.GetIntersection(clipArea.Value) : dest;

            // check if our Image is totally clipped and return if it is
            if ((Math.Abs(finalRect.Width) < float.Epsilon) || (Math.Abs(finalRect.Height) < float.Epsilon))
                return new List<GeometryBuffer>();

            // Calculate the scale factor for our Image which is the scaling of the Image
            // area to the destination area of our render call
            var scaleFactor = new Vector2(dest.Width/d_imageArea.Width, dest.Height/d_imageArea.Height);

            // URGENT FIXME: Shouldn't this be in the hands of the user?
            finalRect.d_min.X = CoordConverter.AlignToPixels(finalRect.d_min.X);
            finalRect.d_min.Y = CoordConverter.AlignToPixels(finalRect.d_min.Y);
            finalRect.d_max.X = CoordConverter.AlignToPixels(finalRect.d_max.X);
            finalRect.d_max.Y = CoordConverter.AlignToPixels(finalRect.d_max.Y);

            var svgRenderSettings = new SvgImageRenderSettings(renderSettings, scaleFactor, d_useGeometryAntialiasing);
            var geometryBuffers = new List<GeometryBuffer>();
            foreach (var shape in d_svgData.getShapes())
                geometryBuffers.AddRange(shape.CreateRenderGeometry(svgRenderSettings));

            return geometryBuffers;
        }

        public override void AddToRenderGeometry(GeometryBuffer geomBuffer, ref Rectf renderArea, ref Rectf? clipArea, ColourRect colours)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the SVGData object of this SVGImage.
        /// </summary>
        /// <param name="svgData">
        /// The pointer to the SVGData object to be used by this SVGImage. Can be 0.
        /// </param>
        public void SetSvgData(SvgData svgData)
        {
            d_svgData = svgData;
        }

        /// <summary>
        /// Returns the pointer to the SVGData object of this SVGImage.
        /// </summary>
        /// <returns>
        /// The pointer to the SVGData object of this SVGImage. Can be 0.
        /// </returns>
        public SvgData GetSvgData()
        {
            return d_svgData;
        }

        /// <summary>
        /// Returns if the rendered geometry of this Image will be extended with geometry
        /// that creates an alpha-blended transition to defeat aliasing artefacts.
        /// </summary>
        /// <returns></returns>
        public bool GetUsesGeometryAntialiasing()
        {
            throw new NotImplementedException();
        }

    /*!
    \brief
        Sets if the rendered geometry will be extended with geometry that creates
        an alpha-blended transition to defeat aliasing artefacts.
    \param use_geometry_antialiasing
        The setting for geometry anti-aliasing that wille be applied to this Image.
    */

        public void setUseGeometryAntialiasing(bool use_geometry_antialiasing)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reference to the SVGData used as basis for drawing. The SVGData can be shared
        /// between SVGImages. Changing the SVGData object will result in changes between all
        /// SVGImages.
        /// </summary>
        protected SvgData d_svgData;

        /// <summary>
        /// Determines if the rendered geometry will be extended with geometry that creates
        /// an alpha-blended transition to defeat aliasing artefacts
        /// </summary>
        protected bool d_useGeometryAntialiasing;

        private const string ImageTypeAttribute = "type";
        private const string ImageNameAttribute = "name";
        private const string ImageSVGDataAttribute = "SVGData";
        private const string ImageXPosAttribute = "xPos";
        private const string ImageYPosAttribute = "yPos";
        private const string ImageWidthAttribute = "width";
        private const string ImageHeightAttribute = "height";
        private const string ImageXOffsetAttribute = "xOffset";
        private const string ImageYOffsetAttribute = "yOffset";
        private const string ImageAutoScaledAttribute = "autoScaled";
        private const string ImageNativeHorzResAttribute = "nativeHorzRes";
        private const string ImageNativeVertResAttribute = "nativeVertRes";
    }
}
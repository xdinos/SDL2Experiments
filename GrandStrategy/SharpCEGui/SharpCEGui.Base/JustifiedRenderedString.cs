using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// FormattedRenderedString implementation that renders the RenderedString with
    /// justified formatting.
    /// </summary>
    public class JustifiedRenderedString: FormattedRenderedString
    {
         //! Constructor.
        public JustifiedRenderedString(){}
        public JustifiedRenderedString(RenderedString @string)
            : base(@string)
        {

        }

        // implementation of base interface
        public override void Format(Window refWnd, Sizef areaSize)
        {
            throw new global::System.NotImplementedException();
        }

        public override List<GeometryBuffer> CreateRenderGeometry(Window refWnd, Lunatics.Mathematics.Vector2 position, ColourRect modColours, Rectf? clipRect)
        {
            var drawPos = position;
            var geomBuffers = new List<GeometryBuffer>();

            for (var i = 0; i < d_renderedString.GetLineCount(); ++i)
            {
                geomBuffers.AddRange(d_renderedString.CreateRenderGeometry(refWnd, i, drawPos, modColours, clipRect, SpaceExtras[i]));
                drawPos.Y += d_renderedString.GetPixelSize(refWnd, i).Height;
            }

            return geomBuffers;
        }

        public override int GetFormattedLineCount()
        {
            throw new global::System.NotImplementedException();
        }

        public override float GetHorizontalExtent(Window refWnd)
        {
            throw new global::System.NotImplementedException();
        }

        public override float GetVerticalExtent(Window refWnd)
        {
            throw new global::System.NotImplementedException();
        }

        /// <summary>
        /// space extra size for each line to achieve justified formatting.
        /// </summary>
        protected List<float> SpaceExtras = new List<float>();
    }
}
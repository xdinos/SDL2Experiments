using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// FormattedRenderedString implementation that renders the RenderedString with
    /// right aligned formatting.
    /// </summary>
    public class RightAlignedRenderedString : FormattedRenderedString
    {
        //! Constructor.
        public RightAlignedRenderedString(){}
        public RightAlignedRenderedString(RenderedString @string)
            : base(@string)
        {

        }

        // implementation of base interface
        public override void Format(Window refWnd, Sizef areaSize)
        {
            d_offsets.Clear();

            for (var i = 0; i < d_renderedString.GetLineCount(); ++i)
                d_offsets.Add(areaSize.Width - d_renderedString.GetPixelSize(refWnd, i).Width);
        }

        public override List<GeometryBuffer> CreateRenderGeometry(Window refWnd, Lunatics.Mathematics.Vector2 position, ColourRect modColours, Rectf? clipRect)
        {
            Lunatics.Mathematics.Vector2 drawPos;
            drawPos.Y = position.Y;
            var geomBuffers = new List<GeometryBuffer>();

            for (var i = 0; i < d_renderedString.GetLineCount(); ++i)
            {
                drawPos.X = position.X + d_offsets[i];
                geomBuffers.AddRange(d_renderedString.CreateRenderGeometry(refWnd, i, drawPos, modColours, clipRect, 0.0f));
                drawPos.Y += d_renderedString.GetPixelSize(refWnd, i).Height;
            }

            return geomBuffers;
        }

        public override int GetFormattedLineCount()
        {
            return d_renderedString.GetLineCount();
        }

        public override float GetHorizontalExtent(Window refWnd)
        {
            var w = 0.0f;
            for (var i = 0; i < d_renderedString.GetLineCount(); ++i)
            {
                var this_width = d_renderedString.GetPixelSize(refWnd, i).Width;
                if (this_width > w)
                    w = this_width;
            }

            return w;
        }

        public override float GetVerticalExtent(Window refWnd)
        {
            var h = 0.0f;
            for (var i = 0; i < d_renderedString.GetLineCount(); ++i)
                h += d_renderedString.GetPixelSize(refWnd, i).Height;

            return h;
        }

        protected List<float> d_offsets = new List<float>();
    }
}
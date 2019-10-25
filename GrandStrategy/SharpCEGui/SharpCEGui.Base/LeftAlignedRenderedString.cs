using System;
using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// FormattedRenderedString implementation that renders the RenderedString with
    /// left aligned formatting.
    /// </summary>
    public class LeftAlignedRenderedString : FormattedRenderedString
    {
        //! Constructor.
        public LeftAlignedRenderedString(){}
        public LeftAlignedRenderedString(RenderedString @string)
            : base(@string)
        {

        }

        // implementation of base interface
        public override void Format(Window refWnd, Sizef areaSize)
        {
        }

        public override List<GeometryBuffer> CreateRenderGeometry(Window refWnd, Lunatics.Mathematics.Vector2 position, ColourRect modColours, Rectf? clipRect)
        {
            var drawPos = position;
            var geomBuffers = new List<GeometryBuffer>();

            for (var i = 0; i < d_renderedString.GetLineCount(); ++i)
            {
                geomBuffers.AddRange(d_renderedString.CreateRenderGeometry(refWnd, i, drawPos, modColours, clipRect, 0.0f));
                drawPos.Y += d_renderedString.GetPixelSize(refWnd, i).Height;
            }

            return geomBuffers;
        }

        public override int GetFormattedLineCount()
        {
            throw new NotImplementedException();
        }

        public override float GetHorizontalExtent(Window refWnd)
        {
            var w = 0.0f;
            for (var i = 0; i < d_renderedString.GetLineCount(); ++i)
            {
                var thisWidth = d_renderedString.GetPixelSize(refWnd, i).Width;
                if (thisWidth > w)
                    w = thisWidth;
            }

            return w;
        }

        public override float GetVerticalExtent(Window refWnd)
        {
            float h = 0.0f;
            for (var i = 0; i < d_renderedString.GetLineCount(); ++i)
                h += d_renderedString.GetPixelSize(refWnd, i).Height;

            return h;
        }
    }
}
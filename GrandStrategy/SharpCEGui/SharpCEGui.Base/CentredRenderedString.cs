using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// FormattedRenderedString implementation that renders the RenderedString with
    /// centred formatting.
    /// </summary>
    public class CentredRenderedString : FormattedRenderedString
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public CentredRenderedString(){}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="string"></param>
        public CentredRenderedString(RenderedString @string)
            : base(@string)
        {

        }

        public override void Format(Window refWnd, Sizef areaSize)
        {
            _offsets.Clear();

            for (var i = 0; i < d_renderedString.GetLineCount(); ++i)
                _offsets.Add(
                    (areaSize.Width - d_renderedString.GetPixelSize(refWnd, i).Width) / 2.0f);
        }

        public override List<GeometryBuffer> CreateRenderGeometry(Window refWnd, Lunatics.Mathematics.Vector2 position, ColourRect modColours, Rectf? clipRect)
        {
            Lunatics.Mathematics.Vector2 drawPos;
            var geomBuffers = new List<GeometryBuffer>();

            drawPos.Y = position.Y;

            for (var i = 0; i < d_renderedString.GetLineCount(); ++i)
            {
                drawPos.X = position.X + _offsets[i];

                var currentRenderGeometry = d_renderedString.CreateRenderGeometry(refWnd, i, drawPos, modColours, clipRect, 0.0f);
                geomBuffers.AddRange(currentRenderGeometry);

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
                var thisWidth = d_renderedString.GetPixelSize(refWnd, i).Width;
                if (thisWidth > w)
                    w = thisWidth;
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

        private readonly List<float> _offsets = new List<float>();
    }
}
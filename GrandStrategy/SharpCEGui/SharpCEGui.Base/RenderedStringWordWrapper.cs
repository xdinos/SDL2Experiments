using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class that handles wrapping of a rendered string into sub-strings.  Each
    /// sub-string is rendered using the FormattedRenderedString based class 'T'.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RenderedStringWordWrapper<T> : FormattedRenderedString where T : FormattedRenderedString, new()
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="string"></param>
        /// <param name="constructor"></param>
        public RenderedStringWordWrapper(RenderedString @string/*, Func<RenderedString, T> constructor*/)
            : base(@string)
        {
            //_constructor = constructor;
        }

        // TODO: Destructor.
        // TODO: ~RenderedStringWordWrapper(){DeleteFormatters();}

        // implementation of base interface

        // TODO: specialised version of format used with Justified text
        public override void Format(Window refWnd, Sizef areaSize)
        {
            DeleteFormatters();

            var lstring = new RenderedString();
            var rstring = new RenderedString(d_renderedString);

            T frs;

            for (var line = 0; line < rstring.GetLineCount(); ++line)
            {
                float rsWidth;
                while ((rsWidth = rstring.GetPixelSize(refWnd, line).Width) > 0)
                {
                    // skip line if no wrapping occurs
                    if (rsWidth <= areaSize.Width)
                        break;

                    // split rstring at width into lstring and remaining rstring
                    rstring.Split(refWnd, line, areaSize.Width, lstring);
                    //frs = _constructor(new RenderedString(lstring));
                    frs = new T();
                    frs.SetRenderedString(new RenderedString(lstring));
                    frs.Format(refWnd, areaSize);
                    Lines.Add(frs);
                    line = 0;
                }
            }

            // last line.
            //frs = _constructor(new RenderedString(rstring));
            frs = new T();
            frs.SetRenderedString(new RenderedString(rstring));
            frs.Format(refWnd, areaSize);
            Lines.Add(frs);
        }

        public override List<GeometryBuffer> CreateRenderGeometry(Window refWnd, Lunatics.Mathematics.Vector2 position, ColourRect modColours, Rectf? clipRect)
        {
            var linePos = position;
            var geomBuffers = new List<GeometryBuffer>();

            foreach (var i in Lines)
            {
                geomBuffers.AddRange(i.CreateRenderGeometry(refWnd, linePos, modColours, clipRect));
                linePos.Y += i.GetVerticalExtent(refWnd);
            }

            return geomBuffers;
        }

        public override int GetFormattedLineCount()
        {
            return Lines.Count;
        }

        public override float GetHorizontalExtent(Window refWnd)
        {
            // TODO: Cache at format time.

            float w = 0;
            foreach (var i in Lines)
            {
                var cur_width = i.GetHorizontalExtent(refWnd);
                if (cur_width > w)
                    w = cur_width;
            }

            return w;
        }

        public override float GetVerticalExtent(Window refWnd)
        {
            // TODO: Cache at format time.

            return Lines.Sum(i => i.GetVerticalExtent(refWnd));
        }

        /// <summary>
        /// Delete the current formatters and associated RenderedStrings
        /// </summary>
        protected void DeleteFormatters()
        {
            for (var i = 0; i < Lines.Count; ++i)
            {
                // get the rendered string back from rthe formatter
                var rs = Lines[i].GetRenderedString();
                
                // delete the formatter
                //TODO: CEGUI_DELETE_AO d_lines[i];
                
                // delete the rendered string.
                //TODO: CEGUI_DELETE_AO rs;
            }

            Lines.Clear();
        }

        //! type of collection used to track the formatted lines.
        //typedef std::vector<FormattedRenderedString* CEGUI_VECTOR_ALLOC(FormattedRenderedString*)> LineList;

        /// <summary>
        /// collection of lines.
        /// </summary>
        protected List<FormattedRenderedString> Lines = new List<FormattedRenderedString>();

        private readonly Func<RenderedString, T> _constructor;
    }
}
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

namespace SharpCEGui.Base.Widgets
{
    /// <summary>
    /// Class used for textual items in a list box.
    /// </summary>
    public class ListboxTextItem : ListboxItem
    {
        /// <summary>
        /// Default text colour.
        /// </summary>
        public static  Colour DefaultTextColour = new Colour(0xFFFFFFFF);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text"></param>
        /// <param name="itemId"></param>
        /// <param name="itemData"></param>
        /// <param name="disabled"></param>
        /// <param name="autoDelete"></param>
        public ListboxTextItem(string text, int itemId = 0, object itemData = null, bool disabled = false,
                               bool autoDelete = true)
            : base(text, itemId, itemData, disabled, autoDelete)
        {
            _textCols = new ColourRect(DefaultTextColour);
            _font = null;
            _renderedStringValid = false;
            _textParsingEnabled = true;
        }

        /// <summary>
        /// Return a pointer to the font being used by this ListboxTextItem
        /// <para>
        /// This method will try a number of places to find a font to be used.  If no font can be
        /// found, NULL is returned.
        /// </para>
        /// </summary>
        /// <returns>
        /// Font to be used for rendering this item
        /// </returns>
        public Font GetFont()
        {
            // prefer out own font
            if (_font != null)
                return _font;

            // try our owner window's font setting (may be null if owner uses no existant default font)
            if (Owner != null)
                return Owner.GetFont();

            // no owner, just use the default (which may be NULL anyway)
            return System.GetSingleton().GetDefaultGUIContext().GetDefaultFont();
        }

        /// <summary>
        /// Return the current colours used for text rendering.
        /// </summary>
        /// <returns>
        /// ColourRect object describing the currently set colours
        /// </returns>
        public ColourRect GetTextColours()
        {
            return _textCols;
        }

        /// <summary>
        /// Set the font to be used by this ListboxTextItem
        /// </summary>
        /// <param name="font">
        /// Font to be used for rendering this item
        /// </param>
        public void SetFont(Font font)
        {
            _font = font;
            _renderedStringValid = false;
        }

        /// <summary>
        /// Set the font to be used by this ListboxTextItem
        /// </summary>
        /// <param name="fontName">
        /// String object containing the name of the Font to be used for rendering this item
        /// </param>
        public void SetFont(string fontName)
        {
            SetFont(FontManager.GetSingleton().Get(fontName));
        }

        /// <summary>
        /// Set the colours used for text rendering.
        /// </summary>
        /// <param name="cols">
        /// ColourRect object describing the colours to be used.
        /// </param>
        public void SetTextColours(ColourRect cols)
        {
            _textCols = cols;
        }

        /// <summary>
        /// Set the colours used for text rendering.
        /// </summary>
        /// <param name="topLeftColour">
        /// Colour (as ARGB value) to be applied to the top-left corner of each text glyph rendered.
        /// </param>
        /// <param name="topRightColour">
        /// Colour (as ARGB value) to be applied to the top-right corner of each text glyph rendered.
        /// </param>
        /// <param name="bottomLeftColour">
        /// Colour (as ARGB value) to be applied to the bottom-left corner of each text glyph rendered.
        /// </param>
        /// <param name="bottomRightColour">
        /// Colour (as ARGB value) to be applied to the bottom-right corner of each text glyph rendered.
        /// </param>
        public void SetTextColours(Colour topLeftColour, Colour topRightColour, Colour bottomLeftColour,
                                   Colour bottomRightColour)
        {
            _textCols.d_top_left = topLeftColour;
            _textCols.d_top_right = topRightColour;
            _textCols.d_bottom_left = bottomLeftColour;
            _textCols.d_bottom_right = bottomRightColour;

            _renderedStringValid = false;
        }

        /// <summary>
        /// Set the colours used for text rendering.
        /// </summary>
        /// <param name="col">
        /// colour value to be used when rendering.
        /// </param>
        public void SetTextColours(Colour col)
        {
            SetTextColours(col, col, col, col);
        }

        /// <summary>
        /// Set whether the the ListboxTextItem will have it's text parsed via the
        /// BasicRenderedStringParser or not.
        /// </summary>
        /// <param name="enable">
        /// - true if the ListboxTextItem text will be parsed.
        /// - false if the ListboxTextItem text will be used verbatim.
        /// </param>
        public void SetTextParsingEnabled(bool enable)
        {
            _textParsingEnabled = enable;
            _renderedStringValid = false;
        }

        /// <summary>
        /// return whether text parsing is enabled for this ListboxTextItem.
        /// </summary>
        /// <returns></returns>
        public bool IsTextParsingEnabled()
        {
            return _textParsingEnabled;
        }


        public override void SetText(string text)
        {
            base.SetText(text);
            _renderedStringValid = false;
        }

        public override bool HandleFontRenderSizeChange(Font font)
        {
            return GetFont() == font;
        }


        /*************************************************************************
            Required implementations of pure virtuals from the base class.
        *************************************************************************/

        public override Sizef GetPixelSize()
        {
            var fnt = GetFont();

            if (fnt == null)
                return Sizef.Zero;

            if (!_renderedStringValid)
                ParseTextString();

            var sz = Sizef.Zero;

            for (var i = 0; i < _renderedString.GetLineCount(); ++i)
            {
                var lineSize = _renderedString.GetPixelSize(Owner, i);
                sz.Height += lineSize.Height;

                if (lineSize.Width > sz.Width)
                    sz.Width = lineSize.Width;
            }

            return sz;
        }
        
        public override List<GeometryBuffer> CreateRenderGeometry(Rectf targetRect, float alpha, Rectf? clipper)
        {
            var geomBuffers = new List<GeometryBuffer>();

            if (Selected && SelectBrush != null)
            {
                var imgRenderSettings = new ImageRenderSettings(targetRect, clipper, true, SelectCols, alpha);
                geomBuffers.AddRange(SelectBrush.CreateRenderGeometry(imgRenderSettings));
            }

            var font = GetFont();

            if (font == null)
                return geomBuffers;

            var drawPos = targetRect.Position;

            drawPos.Y += CoordConverter.AlignToPixels((font.GetLineSpacing() - font.GetFontHeight())*0.5f);

            if (!_renderedStringValid)
                ParseTextString();

            var finalColours = new ColourRect(0xFFFFFFFF);

            for (var i = 0; i < _renderedString.GetLineCount(); ++i)
            {
                geomBuffers.AddRange(_renderedString.CreateRenderGeometry(Owner, i, drawPos, finalColours, clipper, 0.0f));
                drawPos.Y += _renderedString.GetPixelSize(Owner, i).Height;
            }

            return geomBuffers;
        }

        protected void ParseTextString()
        {
            _renderedString = _textParsingEnabled
                                  ? StringParser.Parse(GetTextVisual(), GetFont(), _textCols)
                                  : NoTagsStringParser.Parse(GetTextVisual(), GetFont(), _textCols);

            _renderedStringValid = true;
        }

        #region Fields

        /// <summary>
        /// Colours used for rendering the text.
        /// </summary>
        private ColourRect _textCols;

        /// <summary>
        /// Font used for rendering text.
        /// </summary>
        private Font _font;

        /// <summary>
        /// RenderedString drawn by this item.
        /// </summary>
        private RenderedString _renderedString;

        /// <summary>
        /// boolean used to track when item state changes (and needs re-parse)
        /// </summary>
        private bool _renderedStringValid;

        /// <summary>
        /// boolean that specifies whether text parsing is enabled for the item.
        /// </summary>
        private bool _textParsingEnabled;

        /// <summary>
        /// Parser used to produce a final RenderedString from the standard String.
        /// </summary>
        private static readonly BasicRenderedStringParser StringParser = new BasicRenderedStringParser();

        /// <summary>
        /// Parser used when parsing is off.  Basically just does linebreaks.
        /// </summary>
        private static readonly DefaultRenderedStringParser NoTagsStringParser = new DefaultRenderedStringParser();

        #endregion
    }
}
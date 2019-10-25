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

namespace SharpCEGui.Base
{
    /// <summary>
    /// internal class representing a single font glyph.
    /// <para>
    /// For TrueType fonts initially all FontGlyph's are empty
    /// (getImage() will return 0), but they are filled by demand.
    /// </para>
    /// </summary>
    public class FontGlyph
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="advance"></param>
        /// <param name="image"></param>
        /// <param name="valid"></param>
        public FontGlyph(float advance = 0.0f, Image image = null, bool valid = false)
        {
            _image = image;
            _advance = advance;
            _valid = valid;
        }

        /// <summary>
        /// Return the CEGUI::Image object rendered for this glyph.
        /// </summary>
        /// <returns></returns>
        public Image GetImage()
        {
            return _image;
        }

        /// <summary>
        /// Return the scaled pixel size of the glyph.
        /// </summary>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <returns></returns>
        public Sizef GetSize(float xScale, float yScale)
        {
            return new Sizef(GetWidth(xScale), GetHeight(yScale));
        }

        /// <summary>
        /// Return the scaled width of the glyph.
        /// </summary>
        /// <param name="xScale"></param>
        /// <returns></returns>
        public float GetWidth(float xScale)
        {
            return _image.GetRenderedSize().Width*xScale;
        }

        /// <summary>
        /// Return the scaled height of the glyph.
        /// </summary>
        /// <param name="yScale"></param>
        /// <returns></returns>
        public float GetHeight(float yScale)
        {
            return _image.GetRenderedSize().Height*yScale;
        }

        /// <summary>
        /// Return the rendered advance value for this glyph.
        /// <para>
        /// The rendered advance value is the total number of pixels from the
        /// current pen position that will be occupied by this glyph when rendered.
        /// </para>
        /// </summary>
        /// <param name="nectGlyph"></param>
        /// <param name="xScale"></param>
        /// <returns></returns>
        public virtual float GetRenderedAdvance(FontGlyph nectGlyph, float xScale)
        {
            return (_image.GetRenderedSize().Width +
                    _image.GetRenderedOffset().X)*xScale;
        }

        /// <summary>
        /// Return the horizontal advance value for the glyph.
        /// <para>
        /// The returned value is the number of pixels the pen should move
        /// horizontally to position itself ready to render the next glyph.  This
        /// is not always the same as the glyph image width or rendered advance,
        /// since it allows for horizontal overhangs.
        /// </para>
        /// </summary>
        /// <param name="xScale"></param>
        /// <returns></returns>
        public float GetAdvance(float xScale = 1.0f)
        {
            return _advance*xScale;
        }

        /// <summary>
        /// Set the horizontal advance value for the glyph.
        /// </summary>
        /// <param name="advance"></param>
        public void SetAdvance(float advance)
        {
            _advance = advance;
        }

        /// <summary>
        /// Set the CEGUI::Image object rendered for this glyph.
        /// </summary>
        /// <param name="image"></param>
        public void SetImage(Image image)
        {
            _image = image;
        }

        /// <summary>
        /// mark the FontGlyph as valid
        /// </summary>
        /// <param name="valid"></param>
        public void SetValid(bool valid)
        {
            _valid = valid;
        }

        /// <summary>
        /// return whether the FontGlyph is marked as valid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return _valid;
        }

        #region Fields

        /// <summary>
        /// The image which will be rendered for this glyph.
        /// </summary>
        private Image _image;
        
        /// <summary>
        /// Amount to advance the pen after rendering this glyph
        /// </summary>
        private float _advance;
        
        /// <summary>
        /// says whether this glyph info is actually valid
        /// </summary>
        private bool _valid;

        #endregion
    }
}
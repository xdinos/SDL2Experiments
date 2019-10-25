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

using System;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Dimension type that represents some metric of a Font.
    /// Implements BaseDim interface.
    /// </summary>
    public class FontDim : BaseDim
    {
        // TODO: do we really neeed this...
        // TODO: public FontDim() {}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">
        /// String holding the name suffix of the window to be accessed to obtain
        /// the font and / or text strings to be used when these items are not
        /// explicitly given.
        /// </param>
        /// <param name="font">
        /// String holding the name of the font to use for this dimension.  If the
        /// string is empty, the font assigned to the window passed to getValue will
        /// be used.
        /// </param>
        /// <param name="text">
        /// String holding the text to be measured for horizontal extent. If this
        /// is empty, the text from the window passed to getValue will be used.
        /// </param>
        /// <param name="metric">
        /// One of the FontMetricType values indicating what we should represent.
        /// </param>
        /// <param name="padding">
        /// constant pixel padding value to be added.
        /// </param>
        public FontDim(string name, string font, string text, FontMetricType metric, float padding = 0)
        {
            _font = font;
            _text = text;
            _childName = name;
            _metric = metric;
            _padding = padding;
        }

        /// <summary>
        /// Get the current name of the FontDim.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return _childName;
        }

        /// <summary>
        /// Set the current name of the FontDim.
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            _childName = name;
        }

        /// <summary>
        /// Get the current font of the FontDim.
        /// </summary>
        /// <returns></returns>
        public string GetFont()
        {
            return _font;
        }

        /// <summary>
        /// Set the current font of the FontDim.
        /// </summary>
        /// <param name="font"></param>
        public void SetFont(string font)
        {
            _font = font;
        }

        /// <summary>
        /// Get the current text of the FontDim.
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return _text;
        }

        /// <summary>
        /// Set the current text of the FontDim.
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            _text = text;
        }

        /// <summary>
        /// Get the current metric of the FontDim.
        /// </summary>
        /// <returns></returns>
        public FontMetricType GetMetric()
        {
            return _metric;
        }

        /// <summary>
        /// Set the current metric of the FontDim.
        /// </summary>
        /// <param name="metric"></param>
        public void SetMetric(FontMetricType metric)
        {
            _metric = metric;
        }

        /// <summary>
        /// Get the current padding of the FontDim.
        /// </summary>
        /// <returns></returns>
        public float GetPadding()
        {
            return _padding;
        }

        /// <summary>
        /// Set the current padding of the FontDim.
        /// </summary>
        /// <param name="padding"></param>
        public void SetPadding(float padding)
        {
            _padding = padding;
        }

        public override bool HandleFontRenderSizeChange(Window window, Font font)
        {
            return font == GetFontObject(window);
        }

        // Implementation of the base class interface
        public override float GetValue(Window wnd)
        {
            // get window to use.
            var sourceWindow = String.IsNullOrEmpty(_childName) ? wnd : wnd.GetChild(_childName);

            // get font to use
            var fontObj = GetFontObject(sourceWindow);

            if (fontObj != null)
            {
                switch (_metric)
                {
                    case FontMetricType.LineSpacing:
                        return fontObj.GetLineSpacing() + _padding;

                    case FontMetricType.Baseline:
                        return fontObj.GetBaseline() + _padding;

                    case FontMetricType.HorzExtent:
                        return fontObj.GetTextExtent(String.IsNullOrEmpty(_text) ? sourceWindow.GetText() : _text) +
                               _padding;

                    default:
                        throw new InvalidRequestException("unknown or unsupported FontMetricType encountered.");
                }
            }

            // no font, return padding value only.
            return _padding;
        }

        public override float GetValue(Window wnd, Rectf container)
        {
            return GetValue(wnd);
        }

        public override BaseDim Clone()
        {
            return new FontDim(_childName, _font, _text, _metric, _padding);
        }

        // Implementation of the base class interface
        protected override void WriteXmlElementNameImpl(XMLSerializer xmlStream)
        {
            xmlStream.OpenTag("FontDim");
        }

        protected override void WriteXmlElementAttributesImpl(XMLSerializer xmlStream)
        {
            if (!String.IsNullOrEmpty(_childName))
                xmlStream.Attribute("widget", _childName);

            if (!String.IsNullOrEmpty(_font))
                xmlStream.Attribute("font", _font);

            if (!String.IsNullOrEmpty(_text))
                xmlStream.Attribute("string", _text);

            if (Math.Abs(_padding - 0) > float.Epsilon)
                xmlStream.Attribute("padding", PropertyHelper.ToString(_padding));

            xmlStream.Attribute("type", /*FalagardXMLHelper*/PropertyHelper.ToString(_metric));
        }

        protected Font GetFontObject(Window window)
        {
            return String.IsNullOrEmpty(_font)
                       ? window.GetFont()
                       : FontManager.GetSingleton().Get(_font);
        }

        #region Fields

        /// <summary>
        /// Name of Font.  If empty font will be taken from Window.
        /// </summary>
        private string _font;

        /// <summary>
        /// String to measure for extents, if empty will use window text.
        /// </summary>
        private string _text;

        /// <summary>
        /// String to hold the name of the window to use for fetching missing font and/or text.
        /// </summary>
        private string _childName;

        /// <summary>
        /// what metric we represent.
        /// </summary>
        private FontMetricType _metric;

        /// <summary>
        /// padding value to be added.
        /// </summary>
        private float _padding;

        #endregion
    }
}
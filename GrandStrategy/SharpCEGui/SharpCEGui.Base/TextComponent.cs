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
using System.Collections.Generic;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class that encapsulates information for a text component.
    /// </summary>
    public class TextComponent : ComponentBase
    {
        public TextComponent()
        {
#if CEGUI_BIDI_SUPPORT
            _bidiVisualMapping = new NBidiVisualMapping();
#else
            _bidiVisualMapping = null;
#endif

            _bidiDataValid = false;
            _formattedRenderedString = new LeftAlignedRenderedString(_renderedString);
            _lastHorizontalFormatting = HorizontalTextFormatting.LeftAligned;
            _vertFormatting = new FormattingSetting<VerticalTextFormatting>(VerticalTextFormatting.TopAligned);
            _horzFormatting = new FormattingSetting<HorizontalTextFormatting>(HorizontalTextFormatting.LeftAligned);
        }

        // TODO: ~TextComponent();
        // TODO: TextComponent(const TextComponent& obj);
        // TODO: TextComponent& operator=(const TextComponent& other);

        /// <summary>
        /// Return the text set for this TextComponent.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This returns the text string set directly to the TextComponent,
        /// which may or may not be the actual string that will be used -
        /// since the actual string may be sourced from a property or the main
        /// text string from a window that the TextComponent is rendered to.
        /// To get the actual string, call the getEffectiveText function
        /// instead.
        /// </remarks>
        public string GetText()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return a copy of the actual text string that will be used when rendering this TextComponent.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns></returns>
        public string GetEffectiveText(Window wnd)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// return text string with \e visual ordering of glyphs.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This returns the visual text derived from the string set directly to
        /// the TextComponent, which may or may not be the actual string that
        /// will be used - since the actual string may be sourced from a
        /// property or the main text string from a window that the
        /// TextComponent is rendered to. To get the actual visual string, call
        /// the getEffectiveVisualText function instead.
        /// </remarks>
        public string GetTextVisual()
        {
            // no bidi support
            if (_bidiVisualMapping==null)
                return _textLogical;

            if (!_bidiDataValid)
            {
                _bidiVisualMapping.UpdateVisual(_textLogical);
                _bidiDataValid = true;
            }

            return _bidiVisualMapping.GetTextVisual();
        }

        /// <summary>
        /// Return a copy of the actual text - with visual ordering - that
        /// will be used when rendering this TextComponent.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns></returns>
        public string GetEffectiveVisualText(Window wnd)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the text string for this TextComponent.
        /// </summary>
        /// <param name="text">
        /// String containing text to set on the TextComponent.
        /// </param>
        /// <remarks>
        /// Setting this string may not set the actual string that will be used
        /// when rendering the TextComponent.  The acutal string used will
        /// depend upon whether a text source property is set and whether this
        /// string is set to the empty string or not.
        /// </remarks>
        public void SetText(string text)
        {
            _textLogical = text;
            _bidiDataValid = false;
        }

        /*!
        \brief
            Return the name of the font set to be used when rendering this
            TextComponent.

        \note
            This returns the name of the font set directly to the TextComponent,
            which may or may not be the actual font that will be used -
            since the actual font may be sourced from a property or the main
            font setting on a window that the TextComponent is rendered to, or
            the default font. To get the actual font name that will be used,
            call the getEffectiveFont function instead.

        \return
            String object containing the name of a font
        */
        public string GetFont()
        {
            throw new NotImplementedException();
        }

        /*
        \brief
            Return a copy of the name of the font that will actually be used
            when rendering this TextComponent.
        */
        public string GetEffectiveFont(Window wnd)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the name of a font to be used when rendering this TextComponent.
        /// </summary>
        /// <param name="font">
        /// String containing name of a font
        /// </param>
        /// <remarks>
        /// Setting this may not set the actual font that will be used
        /// when rendering the TextComponent.  The acutal font used will
        /// depend upon whether a font source property is set and whether the
        /// font name set here is set to the empty string or not.
        /// </remarks>
        public void SetFont(string font)
        {
            _font = font;
        }

        /// <summary>
        /// Return the current vertical formatting setting for this TextComponent.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns>
        /// One of the VerticalTextFormatting enumerated values.
        /// </returns>
        public VerticalTextFormatting GetVerticalFormatting(Window wnd)
        {
            return _vertFormatting.Get(wnd);
        }

        /// <summary>
        /// Set the vertical formatting setting for this TextComponent.
        /// </summary>
        /// <param name="fmt">
        /// One of the VerticalTextFormatting enumerated values.
        /// </param>
        public void SetVerticalFormatting(VerticalTextFormatting fmt)
        {
            _vertFormatting.Set(fmt);
        }

        /*!
        \brief
            Return the current horizontal formatting setting for this TextComponent.

        \return
            One of the HorizontalTextFormatting enumerated values.
        */
        public HorizontalTextFormatting GetHorizontalFormatting(Window wnd)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the horizontal formatting setting for this TextComponent.
        /// </summary>
        /// <param name="fmt">
        /// One of the HorizontalTextFormatting enumerated values.
        /// </param>
        public void SetHorizontalFormatting(HorizontalTextFormatting fmt)
        {
            _horzFormatting.Set(fmt);
        }

        /// <summary>
        /// Set the name of a property that will be used to obtain the horizontal
        /// formatting to use for this ImageryComponent.
        /// </summary>
        /// <param name="propertyName"></param>
        public void SetHorizontalFormattingPropertySource(string propertyName)
        {
            _horzFormatting.SetPropertySource(propertyName);
        }

        /// <summary>
        /// Set the name of a property that will be used to obtain the vertical
        /// formatting to use for this ImageryComponent.
        /// </summary>
        /// <param name="propertyName"></param>
        public void SetVerticalFormattingPropertySource(string propertyName)
        {
            _vertFormatting.SetPropertySource(propertyName);
        }

        /*!
        \brief
            Writes an xml representation of this TextComponent to \a out_stream.

        \param xml_stream
            Stream where xml data should be output.


        \return
            Nothing.
        */
        public void WriteXMLToStream(XMLSerializer xml_stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return whether this TextComponent fetches it's text string via a property on the target window.
        /// </summary>
        /// <returns>
        /// - true if the text string comes via a Propery.
        /// - false if the text string is defined explicitly, or will come from the target window.
        /// </returns>
        public bool IsTextFetchedFromProperty()
        {
            return !String.IsNullOrEmpty(_textPropertyName);
        }

        /// <summary>
        /// Return the name of the property that will be used to determine the text string to render
        /// for this TextComponent.
        /// </summary>
        /// <returns>
        /// String object holding the name of a Propery.
        /// </returns>
        public string GetTextPropertySource()
        {
            return _textPropertyName;
        }

        /// <summary>
        /// Set the name of the property that will be used to determine the text string to render
        /// for this TextComponent.
        /// </summary>
        /// <param name="property">
        /// String object holding the name of a Property.  The property can contain any text string to render.
        /// </param>
        public void SetTextPropertySource(string property)
        {
            _textPropertyName = property;
        }
        
        /*!
        \brief
            Return whether this TextComponent fetches it's font via a property on the target window.

        \return
            - true if the font comes via a Propery.
            - false if the font is defined explicitly, or will come from the target window.
        */
        public bool IsFontFetchedFromProperty()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Return the name of the property that will be used to determine the font to use for rendering
            the text string for this TextComponent.

        \return
            String object holding the name of a Propery.
        */
        public string GetFontPropertySource()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Set the name of the property that will be used to determine the font to use for rendering
            the text string of this TextComponent.

        \param property
            String object holding the name of a Propery.  The property should access a valid font name.

        \return
            Nothing.
        */
        public void SetFontPropertySource(string property)
        {
            _fontPropertyName = property;
        }
        
        /// <summary>
        /// return the horizontal pixel extent of the formatted rendered string.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public float GetHorizontalTextExtent(Window window)
        {
            UpdateFormatting(window);
            return _formattedRenderedString.GetHorizontalExtent(window);
        }

        /// <summary>
        /// return the vertical pixel extent of the formatted rendered string.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public float GetVerticalTextExtent(Window window)
        {
            UpdateFormatting(window);
            return _formattedRenderedString.GetVerticalExtent(window);
        }
    
        // overridden from ComponentBase.
        public override bool HandleFontRenderSizeChange(Window window, Font font)
        {
            var res =  base.HandleFontRenderSizeChange(window, font);

            if (font == GetFontObject(window))
            {
                window.Invalidate(false);
                return true;
            }

            return res;
        }

        /// <summary>
        /// Update string formatting.
        /// </summary>
        /// <param name="srcWindow"></param>
        public void UpdateFormatting(Window srcWindow)
        {
            UpdateFormatting(srcWindow, d_area.GetPixelRect(srcWindow).Size);   
        }

        /// <summary>
        /// Update string formatting.
        /// </summary>
        /// <param name="srcWindow"></param>
        /// <param name="size">
        /// The pixel size of the component.
        /// </param>
        public void UpdateFormatting(Window srcWindow, Sizef size)
        {
            var font = GetFontObject(srcWindow);

            // exit if we have no font to use.
            if (font == null)
                throw new InvalidRequestException("Window doesn't have a font.");

            var rs = _renderedString;
            // do we fetch text from a property
            if (!String.IsNullOrEmpty(_textPropertyName))
            {
                // fetch text & do bi-directional reordering as needed
                string visual;
#if CEGUI_BIDI_SUPPORT
                var logicalToVisual = new List<int>();
                var visualToLogical = new List<int>();
                _bidiVisualMapping.ReorderFromLogicalToVisual(srcWindow.GetProperty(_textPropertyName), out visual, logicalToVisual, visualToLogical);
#else
                visual = srcWindow.GetProperty(_textPropertyName);
#endif
                // parse string using parser from Window.
                _renderedString = srcWindow.GetRenderedStringParser().Parse(visual, font, null);
                rs = _renderedString;
            }
            // do we use a static text string from the looknfeel
            else if (!String.IsNullOrEmpty(GetTextVisual()))
            {
                // parse string using parser from Window.
                _renderedString = srcWindow.GetRenderedStringParser().Parse(GetTextVisual(), font, null);
            }
            // do we have to override the font?
            else if (font != srcWindow.GetFont())
            {
                _renderedString = srcWindow.GetRenderedStringParser().Parse(srcWindow.GetTextVisual(), font, null);
            }
            // use ready-made RenderedString from the Window itself
            else
            {
                rs = srcWindow.GetRenderedString();
            }

            SetupStringFormatter(srcWindow, rs);
            _formattedRenderedString.Format(srcWindow, size);
        }

        // implemets abstract from base
        protected override void AddImageRenderGeometryToWindowImpl(Window srcWindow, Rectf destRect, ColourRect modColours, Rectf? clipper, bool clipToDisplay)
        {
            UpdateFormatting(srcWindow, destRect.Size);

            // Get total formatted height.
            var textHeight = _formattedRenderedString.GetVerticalExtent(srcWindow);

            // handle dest area adjustments for vertical formatting.
            var vertFormatting = _vertFormatting.Get(srcWindow);

            switch (vertFormatting)
            {
                case VerticalTextFormatting.CentreAligned:
                    destRect.d_min.Y += (destRect.Height - textHeight)*0.5f;
                    break;

                case VerticalTextFormatting.BottomAligned:
                    destRect.d_min.Y = destRect.d_max.Y - textHeight;
                    break;
            }

            // calculate final colours to be used
            ColourRect finalColours;
            InitColoursRect(srcWindow, modColours, out finalColours);

            // add geometry for text to the target window.
            var geomBuffers = _formattedRenderedString.CreateRenderGeometry(srcWindow, destRect.Position, finalColours, clipper);
            srcWindow.AppendGeometryBuffers(geomBuffers);
        }

        /// <summary>
        /// helper to set up an appropriate FormattedRenderedString
        /// </summary>
        /// <param name="window"></param>
        /// <param name="renderedString"></param>
        protected void SetupStringFormatter(Window window, RenderedString renderedString)
        {
            var horzFormatting = _horzFormatting.Get(window);

            // no formatting change
            if (horzFormatting == _lastHorizontalFormatting)
            {
                _formattedRenderedString.SetRenderedString(renderedString);
                return;
            }

            _lastHorizontalFormatting = horzFormatting;

            switch(horzFormatting)
            {
            case HorizontalTextFormatting.LeftAligned:
                    _formattedRenderedString = new LeftAlignedRenderedString(renderedString);
                break;

            case HorizontalTextFormatting.CentreAligned:
                _formattedRenderedString = new CentredRenderedString(renderedString);
                break;

            case HorizontalTextFormatting.RightAligned:
                _formattedRenderedString = new RightAlignedRenderedString(renderedString);
                break;

            case HorizontalTextFormatting.Justified:
                _formattedRenderedString = new JustifiedRenderedString(renderedString);
                break;

            case HorizontalTextFormatting.WordWrapLeftAligned:
                    _formattedRenderedString =
                        new RenderedStringWordWrapper<LeftAlignedRenderedString>(renderedString);
                        //new RenderedStringWordWrapper<LeftAlignedRenderedString>(rendered_string, rs => new LeftAlignedRenderedString(rs));
                break;

            case HorizontalTextFormatting.WordWrapCentreAligned:
                    _formattedRenderedString =
                        new RenderedStringWordWrapper<CentredRenderedString>(renderedString);
                        //new RenderedStringWordWrapper<CentredRenderedString>(rendered_string, rs => new CentredRenderedString(rs));
                break;

            case HorizontalTextFormatting.WordWrapRightAligned:
                    _formattedRenderedString =
                        new RenderedStringWordWrapper<RightAlignedRenderedString>(renderedString);
                        //new RenderedStringWordWrapper<RightAlignedRenderedString>(rendered_string, rs => new RightAlignedRenderedString(rs));
                break;

            case HorizontalTextFormatting.WordWrapJustified:
                    _formattedRenderedString = 
                        new RenderedStringWordWrapper<JustifiedRenderedString>(renderedString);
                        //new RenderedStringWordWrapper<JustifiedRenderedString>(rendered_string, rs => new JustifiedRenderedString(rs));
                break;
            }
        }

        /// <summary>
        /// helper to get the font object to use
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        protected Font GetFontObject(Window window)
        {
            try
            {
                return String.IsNullOrEmpty(_fontPropertyName)
                           ? (String.IsNullOrEmpty(_font)
                                  ? window.GetFont()
                                  : FontManager.GetSingleton().Get(_font))
                           : FontManager.GetSingleton().Get(window.GetProperty(_fontPropertyName));
            }
            catch (UnknownObjectException)
            {
                return null;
            }
        }

        #region Fields

        /// <summary>
        /// text rendered by this component.
        /// </summary>
        private string _textLogical;

        /// <summary>
        /// pointer to bidirection support object
        /// </summary>
        private readonly BidiVisualMapping _bidiVisualMapping;

        /// <summary>
        /// whether bidi visual mapping has been updated since last text change.
        /// </summary>
        private bool _bidiDataValid;

        /// <summary>
        /// RenderedString used when not using the one from the target Window.
        /// </summary>
        private RenderedString _renderedString;

        // TODO: private RefCounted<FormattedRenderedString> d_formattedRenderedString;
        /// <summary>
        /// FormattedRenderedString object that applies formatting to the string
        /// </summary>
        private FormattedRenderedString _formattedRenderedString;

        /// <summary>
        /// Tracks last used horizontal formatting (in order to detect changes)
        /// </summary>
        private HorizontalTextFormatting _lastHorizontalFormatting;

        /// <summary>
        /// name of font to use.
        /// </summary>
        private string _font;

        /// <summary>
        /// Vertical formatting to be applied when rendering the image component.
        /// </summary>
        private readonly FormattingSetting<VerticalTextFormatting> _vertFormatting;
        
        /// <summary>
        /// Horizontal formatting to be applied when rendering the image component.
        /// </summary>
        private readonly FormattingSetting<HorizontalTextFormatting> _horzFormatting;
        
        /// <summary>
        /// Name of the property to access to obtain the text string to render.
        /// </summary>
        private string  _textPropertyName;
        
        /// <summary>
        /// Name of the property to access to obtain the font to use for rendering.
        /// </summary>
        private string  _fontPropertyName;

        #endregion
    }
}
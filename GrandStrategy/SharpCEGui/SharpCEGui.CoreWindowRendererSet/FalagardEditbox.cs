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
using System.Globalization;
using SharpCEGui.Base;
using SharpCEGui.Base.Widgets;

namespace SharpCEGui.CoreWindowRendererSet
{
    /// <summary>
    /// Editbox class for the FalagardBase module.
    /// 
    /// This class requires LookNFeel to be assigned.  The LookNFeel should provide
    /// the following:
    /// 
    /// States:
    ///     - Enabled: Rendering for when the editbox is in enabled and is in
    ///                read-write mode.
    ///     - ReadOnly: Rendering for when the editbox is in enabled and is in
    ///                read-only mode.
    ///     - Disabled: Rendering for when the editbox is disabled.
    ///     - ActiveSelection: additional state rendered for text selection
    ///                        (the imagery in this section is rendered within the
    ///                        selection area.)
    ///     - InactiveSelection: additional state rendered for text selection
    ///                          (the imagery in this section is rendered within the
    ///                          selection area.)
    /// 
    /// NamedAreas: 
    ///     - TextArea: area where text, selection, and caret imagery will appear.
    /// 
    /// PropertyDefinitions (optional)
    ///     - NormalTextColour: property that accesses a colour value to be used to
    ///                         render normal unselected text.  If this property is
    ///                         not defined, the colour defaults to black.
    ///     - SelectedTextColour: property that accesses a colour value to be used
    ///                           to render selected text.  If this property is
    ///                           not defined, the colour defaults to black.
    /// 
    /// Imagery Sections:
    ///     - Caret
    /// </summary>
    public class FalagardEditbox: EditboxWindowRenderer
    {
        /// <summary>
        /// type name for this widget.
        /// </summary>
        public const string TypeName = "Core/Editbox";

        /// <summary>
        /// Name of property to access for unselected text colour.
        /// </summary>
        public const string UnselectedTextColourPropertyName = "NormalTextColour";

        /// <summary>
        /// Name of property to access for selected text colour.
        /// </summary>
        public const string SelectedTextColourPropertyName = "SelectedTextColour";

        /// <summary>
        /// The default timeout (in seconds) used when blinking the caret.
        /// </summary>
        public const float DefaultCaretBlinkTimeout = 0.66f;

        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public FalagardEditbox(string type):base(type)
        {
            _lastTextOffset = 0;
            _blinkCaret = false;
            _caretBlinkTimeout = DefaultCaretBlinkTimeout;
            _caretBlinkElapsed = 0.0f;
            _showCaret = true;
            _textFormatting = HorizontalTextFormatting.LeftAligned;

            RegisterProperty(new TplWindowRendererProperty<FalagardEditbox, bool>(
                                 "BlinkCaret",
                                 "Property to get/set whether the Editbox caret should blink. Value is either \"True\" or \"False\".",
                                 (w, v) => w.SetCaretBlinkEnabled(v), w => w.IsCaretBlinkEnabled(), TypeName));

            RegisterProperty(new TplWindowRendererProperty<FalagardEditbox, float>(
                                 "BlinkCaretTimeout",
                                 "Property to get/set the caret blink timeout / speed. Value is a float value indicating the timeout in seconds.",
                                 (w, v) => w.SetCaretBlinkTimeout(v), w => w.GetCaretBlinkTimeout(), TypeName,
                                 DefaultCaretBlinkTimeout));

            RegisterProperty(new TplWindowRendererProperty<FalagardEditbox, HorizontalTextFormatting>(
                                 "TextFormatting",
                                 "Property to get/set the horizontal formatting mode. Value is one of: LeftAligned, RightAligned or CentreAligned",
                                 (w, v) => w.SetTextFormatting(v), w => GetTextFormatting(), TypeName));
        }

        /// <summary>
        /// Set the given ColourRect to the colour to be used for rendering Editbox
        /// text oustside of the selected region.
        /// </summary>
        /// <param name="colourRect"></param>
        public void SetColourRectToUnselectedTextColour(ref ColourRect colourRect)
        {
            SetColourRectToOptionalPropertyColour(UnselectedTextColourPropertyName, ref colourRect);
        }

        /// <summary>
        /// Set the given ColourRect to the colour to be used for rendering Editbox
        /// text falling within the selected region.
        /// </summary>
        /// <param name="colourRect"></param>
        public void SetColourRectToSelectedTextColour(ref ColourRect colourRect)
        {
            SetColourRectToOptionalPropertyColour(SelectedTextColourPropertyName, ref colourRect);
        }

        /// <summary>
        /// Set the given ColourRect to the colour(s) fetched from the named
        /// property if it exists, else the default colour of black.
        /// </summary>
        /// <param name="propertyName">
        /// String object holding the name of the property to be accessed if it
        /// exists.
        /// </param>
        /// <param name="colourRect">
        /// Reference to a ColourRect that will be set.
        /// </param>
        public void SetColourRectToOptionalPropertyColour(string propertyName, ref ColourRect colourRect)
        {
            if (Window.IsPropertyPresent(propertyName))
                colourRect = Window.GetProperty<ColourRect>(propertyName);
            else
                colourRect.SetColours(new Colour(0));
        }

        /// <summary>
        /// return whether the blinking caret is enabled.
        /// </summary>
        /// <returns></returns>
        public bool IsCaretBlinkEnabled()
        {
            return _blinkCaret;
        }

        /// <summary>
        /// return the caret blink timeout period (only used if blink is enabled).
        /// </summary>
        /// <returns></returns>
        public float GetCaretBlinkTimeout()
        {
            return _caretBlinkTimeout;
        }

        /// <summary>
        /// set whether the blinking caret is enabled.
        /// </summary>
        /// <param name="enable"></param>
        public void SetCaretBlinkEnabled(bool enable)
        {
            _blinkCaret = enable;
        }

        /// <summary>
        /// set the caret blink timeout period (only used if blink is enabled).
        /// </summary>
        /// <param name="seconds"></param>
        public void SetCaretBlinkTimeout(float seconds)
        {
            _caretBlinkTimeout = seconds;
        }

        /// <summary>
        /// Sets the horizontal text formatting to be used from now onwards.
        /// </summary>
        /// <param name="format">
        /// Specifies the formatting to use.  Currently can only be one of the
        /// following HorizontalTextFormatting values:
        ///     - HTF_LEFT_ALIGNED (default)
        ///     - HTF_RIGHT_ALIGNED
        ///     - HTF_CENTRE_ALIGNED
        /// </param>
        public void SetTextFormatting(HorizontalTextFormatting format)
        {
            if (IsUnsupportedFormat(format))
                throw new InvalidRequestException(
                    "currently only HTF_LEFT_ALIGNED, HTF_RIGHT_ALIGNED and HTF_CENTRE_ALIGNED are accepted for Editbox formatting");

            _textFormatting = format;
            Window.Invalidate(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public HorizontalTextFormatting GetTextFormatting()
        {
            return _textFormatting;
        }

        public override void CreateRenderGeometry()
        {
            var wlf = GetLookNFeel();

            RenderBaseImagery(wlf);

            // no font == no more rendering
            var font = Window.GetFont();
            if (font == null)
                return;

            string visualText;
            SetupVisualString(out visualText);

            var caretImagery = wlf.GetImagerySection("Caret");

            // get destination area for text
            var textArea = wlf.GetNamedArea("TextArea").GetArea().GetPixelRect(Window);

            var caretIndex = GetCaretIndex(visualText);
            var extentToCaret = font.GetTextAdvance(visualText.CEGuiSubstring(0, caretIndex));
            var caretWidth = caretImagery.GetBoundingRect(Window, textArea).Width;
            var textExtent = font.GetTextExtent(visualText);
            var textOffset = CalculateTextOffset(textArea, textExtent, caretWidth, extentToCaret);

#if CEGUI_BIDI_SUPPORT
            CreateRenderGeometryForTextWithBidi(wlf, visualText, textArea, textOffset);
#else
            CreateRenderGeometryForTextWithoutBidi(wlf, visualText, textArea, textOffset);
#endif

            // remember this for next time.
            _lastTextOffset = textOffset;

            RenderCaret(caretImagery, textArea, textOffset, extentToCaret);
        }

        // overridden from EditboxWindowRenderer base class.
        public override int GetTextIndexFromPosition(Lunatics.Mathematics.Vector2 pt)
        {
            var w = (Editbox) Window;

            // calculate final window position to be checked
            var wndx = CoordConverter.ScreenToWindowX(w, pt.X);

            wndx -= _lastTextOffset;

            // Return the proper index
            return w.GetFont().GetCharAtPixel(w.IsTextMaskingEnabled()
                                                  ? new string(w.GetTextMaskCodePoint(), w.GetTextVisual().Length)
                                                  : w.GetTextVisual(), wndx);
        }

        // overridden from WindowRenderer class
        public override void Update(float elapsed)
        {
            // do base class stuff
            base.Update(elapsed);

            // only do the update if we absolutely have to
            if (_blinkCaret &&
                !((Editbox) Window).IsReadOnly() &&
                ((Editbox) Window).HasInputFocus())
            {
                _caretBlinkElapsed += elapsed;

                if (_caretBlinkElapsed > _caretBlinkTimeout)
                {
                    _caretBlinkElapsed = 0.0f;
                    _showCaret ^= true;
                    // state changed, so need a redraw
                    Window.Invalidate(false);
                }
            }
        }

        public override bool HandleFontRenderSizeChange(Font font)
        {
            var res = base.HandleFontRenderSizeChange(font);

            if (Window.GetFont() == font)
            {
                Window.Invalidate(false);
                return true;
            }

            return res;
        }

        /// <summary>
        /// helper to draw the base imagery (container and what have you)
        /// </summary>
        /// <param name="wlf"></param>
        protected void RenderBaseImagery(WidgetLookFeel wlf)
        {
            var w = (Editbox) Window;

            var imagery = wlf.GetStateImagery(w.IsEffectiveDisabled()
                                                  ? "Disabled"
                                                  : (w.IsReadOnly() ? "ReadOnly" : "Enabled"));

            imagery.Render(w);
        }

        /// <summary>
        /// helper to set 'visual' to the string we will render (part of)
        /// </summary>
        /// <param name="visual"></param>
        protected void SetupVisualString(out string visual)
        {
            var w = (Editbox) Window;

            visual = w.IsTextMaskingEnabled()
                         ? new string(w.GetTextMaskCodePoint(), w.GetText().Length)
                         : w.GetTextVisual();
        }

        protected int GetCaretIndex(string visualString)
        {
            var w = (Editbox) Window;

            var caretIndex = w.GetCaretIndex();

        #if CEGUI_BIDI_SUPPORT
            // the char before the caret bidi type
            var currCharIsRtl = false;
            if (!String.IsNullOrEmpty(visualString) && caretIndex > 0)
            {
                var curCaretIndex = w.GetCaretIndex();
                var charBeforeCaretType = w.GetBidiVisualMapping().GetBidiCharType(visualString[curCaretIndex - 1]);
                // for neutral chars you decide by the char after
                for (; BidiCharType.BCT_NEUTRAL == charBeforeCaretType && (visualString.Length > curCaretIndex); curCaretIndex++)
                {
                    charBeforeCaretType = w.GetBidiVisualMapping().GetBidiCharType(visualString[curCaretIndex - 1]);
                }

                currCharIsRtl  = (BidiCharType.BCT_RIGHT_TO_LEFT == charBeforeCaretType);
            }

            var isFirstChar = caretIndex == 0;

            // the pos is by the char before
            if (!isFirstChar)
                caretIndex--;

            // we need to find the caret pos by the logical to visual map
            if (w.GetBidiVisualMapping().GetV2lMapping().Count > caretIndex)
                caretIndex = w.GetBidiVisualMapping().GetV2lMapping()[caretIndex];

            // for non RTL char - the caret pos is after the char
            if (!currCharIsRtl)
                caretIndex++;

            // if first char is not rtl - we need to stand at the start of the line
            if (isFirstChar)
            {
                var firstCharRtl = !String.IsNullOrEmpty(visualString) &&
                                   (BidiCharType.BCT_RIGHT_TO_LEFT == w.GetBidiVisualMapping().GetBidiCharType(visualString[0]));

                if (!firstCharRtl)
                    caretIndex--;
            }
        #endif

            return caretIndex;
        }

        protected float CalculateTextOffset(Rectf textArea,
                                            float textExtent,
                                            float caretWidth,
                                            float extentToCaret)
        {
            // if caret is to the left of the box
            if ((_lastTextOffset + extentToCaret) < 0)
                return -extentToCaret;

            // if caret is off to the right.
            if ((_lastTextOffset + extentToCaret) >= (textArea.Width - caretWidth))
                return textArea.Width - extentToCaret - caretWidth;

            // handle formatting of text when it's shorter than the available space
            if (textExtent < textArea.Width)
            {
                if (_textFormatting == HorizontalTextFormatting.CentreAligned)
                    return (textArea.Width - textExtent)/2;

                if (_textFormatting == HorizontalTextFormatting.RightAligned)
                    return textArea.Width - textExtent;
            }

            // no change to text position; re-use last offset value.
            return _lastTextOffset;
        }

        protected void CreateRenderGeometryForTextWithoutBidi(WidgetLookFeel wlf, string text, Rectf textArea, float textOffset)
        {
            var font = Window.GetFont();

            // setup initial rect for text formatting
            var textPartRect = textArea;

            // allow for scroll position
            textPartRect.d_min.X += textOffset;
            
            // centre text vertically within the defined text area
            textPartRect.d_min.Y += (textArea.Height - font.GetFontHeight()) * 0.5f;

            var alphaComp = Window.GetEffectiveAlpha();
            
            // get unhighlighted text colour (saves accessing property twice)
            var unselectedColours=new ColourRect();
            SetColourRectToUnselectedTextColour(ref unselectedColours);
            
            // see if the editbox is active or inactive.
            var w = (Editbox) Window;
            var active = EditboxIsFocussed();

            if (w.GetSelectionLength() != 0)
            {
                // calculate required start and end offsets of selection imagery.
                var selStartOffset = font.GetTextAdvance(text.CEGuiSubstring(0, w.GetSelectionStart()));
                var selEndOffset = font.GetTextAdvance(text.CEGuiSubstring(0, w.GetSelectionEnd()));

                // calculate area for selection imagery.
                Rectf hlarea = textArea;
                hlarea.d_min.X += textOffset + selStartOffset;
                hlarea.d_max.X = hlarea.d_min.X + (selEndOffset - selStartOffset);

                // create render geometry for the selection imagery.
                wlf.GetStateImagery(active ? "ActiveSelection" : "InactiveSelection").Render(w, hlarea, null, textArea);
            }

            // create render geometry for pre-highlight text
            var sect = text.CEGuiSubstring(0, w.GetSelectionStart());
            var colours = unselectedColours;
            colours.ModulateAlpha(alphaComp);
            var preHighlightTextGeomBuffers = font.CreateRenderGeometryForText(sect,out textPartRect.d_min.X, textPartRect.Position, textArea, true, colours);
            w.AppendGeometryBuffers(preHighlightTextGeomBuffers);

            // create render geometry for highlight text
            sect = text.CEGuiSubstring(w.GetSelectionStart(), w.GetSelectionLength());
            SetColourRectToSelectedTextColour(ref colours);
            colours.ModulateAlpha(alphaComp);
            var highlitTextGeomBuffers = font.CreateRenderGeometryForText(sect, out textPartRect.d_min.X, textPartRect.Position, textArea, true, colours);
            w.AppendGeometryBuffers(highlitTextGeomBuffers);

            // create render geometry for post-highlight text
            sect = text.Substring(w.GetSelectionEnd());
            colours = unselectedColours;
            colours.ModulateAlpha(alphaComp);
            var postHighlitTextGeomBuffers = font.CreateRenderGeometryForText(sect, out textPartRect.d_min.X, textPartRect.Position, textArea, true, colours);
            w.AppendGeometryBuffers(postHighlitTextGeomBuffers);
        }

        protected void CreateRenderGeometryForTextWithBidi(WidgetLookFeel wlf, string text, Rectf textArea, float textOffset)
        {
            var font = Window.GetFont();

            // setup initial rect for text formatting
            var textPartRect = textArea;
            // allow for scroll position
            textPartRect.d_min.X += textOffset;
            // centre text vertically within the defined text area
            textPartRect.d_min.Y += (textArea.Height - font.GetFontHeight()) * 0.5f;

            var colours=new ColourRect();
            var alphaComp = Window.GetEffectiveAlpha();
            // get unhighlighted text colour (saves accessing property twice)
            var unselectedColour = new ColourRect();
            SetColourRectToUnselectedTextColour(ref unselectedColour);
            // see if the editbox is active or inactive.
            var w = (Editbox) Window;
            var active = EditboxIsFocussed();

            if (w.GetSelectionLength() == 0)
            {
                // no highlighted text - we can draw the whole thing
                colours = unselectedColour;
                w.AppendGeometryBuffers(font.CreateRenderGeometryForText(text, out textPartRect.d_min.X, textPartRect.Position, textArea, true, colours));
            }
            else
            {
                // there is highlighted text - because of the Bidi support - the
                // highlighted area can be in some cases nonconsecutive.
                // So - we need to draw it char by char (I guess we can optimize it more
                // but this is not that big performance hit because it only happens if
                // we have highlighted text - not that common...)
                for (var i = 0 ; i < text.Length ; i++)
                {
                    // get the char
                    var currChar = text[i];
                    var realPos = 0;

                    // get he visual pos of the char
                    if (w.GetBidiVisualMapping().GetV2lMapping().Count > i)
                    {
                        realPos = w.GetBidiVisualMapping().GetV2lMapping()[i];
                    }

                    // check if it is in the highlighted region
                    var highlighted = realPos >= w.GetSelectionStart() &&
                                      realPos < w.GetSelectionStart() + w.GetSelectionLength();

                    var charAdvance = font.GetGlyphData(currChar).GetAdvance();

                    if (highlighted)
                    {
                        SetColourRectToSelectedTextColour(ref colours);
                        colours.ModulateAlpha(alphaComp);

                        {

                            // calculate area for selection imagery.
                            var hlarea =textArea;
                            hlarea.d_min.X = textPartRect.d_min.X ;
                            hlarea.d_max.X = textPartRect.d_min.X + charAdvance ;

                            // render the selection imagery.
                            wlf.GetStateImagery(active ? "ActiveSelection" :"InactiveSelection").Render(w, hlarea, null, textArea);
                        }

                    }
                    else
                    {
                        colours = unselectedColour;
                        colours.ModulateAlpha(alphaComp);
                    }
                    
                    w.AppendGeometryBuffers(font.CreateRenderGeometryForText(currChar.ToString(CultureInfo.InvariantCulture), textPartRect.Position, textArea, true, colours));

                    // adjust rect for next section
                    textPartRect.d_min.X += charAdvance;

                }
            }
        }

        protected bool EditboxIsFocussed()
        {
            return ((Editbox)Window).HasInputFocus();
        }

        protected bool EditboxIsReadOnly()
        {
            return ((Editbox) Window).IsReadOnly();
        }

        protected void RenderCaret(ImagerySection imagery,
                                   Rectf textArea,
                                   float textOffset,
                                   float extentToCaret)
        {
            if ((!_blinkCaret || _showCaret) && EditboxIsFocussed() && !EditboxIsReadOnly())
            {
                var caretRect = textArea;
                caretRect.d_min.X += extentToCaret + textOffset;

                imagery.Render(Window, caretRect, null, textArea);
            }
        }

        private static bool IsUnsupportedFormat(HorizontalTextFormatting format)
        {
            return !(format == HorizontalTextFormatting.LeftAligned ||
                     format == HorizontalTextFormatting.RightAligned ||
                     format == HorizontalTextFormatting.CentreAligned);
        }

        #region Fields

        /// <summary>
        /// x rendering offset used last time we drew the widget.
        /// </summary>
        private float _lastTextOffset;

        /// <summary>
        /// true if the caret imagery should blink.
        /// </summary>
        private bool _blinkCaret;

        /// <summary>
        /// time-out in seconds used for blinking the caret.
        /// </summary>
        private float _caretBlinkTimeout;

        /// <summary>
        /// current time elapsed since last caret blink state change.
        /// </summary>
        private float _caretBlinkElapsed;

        /// <summary>
        /// true if caret should be shown.
        /// </summary>
        private bool _showCaret;

        /// <summary>
        /// horizontal formatting.  Only supports left, right, and centred.
        /// </summary>
        private HorizontalTextFormatting _textFormatting;

        #endregion
    }
}